﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Damselfly.Core.Models;
using Damselfly.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Damselfly.Core.DbModels;

namespace Damselfly.Core.Services
{
    /// <summary>
    /// Singleton service which is responsible for maintaining the selection
    /// of images saved in the 'basket' for export, sharing, upload and other p
    /// rocessing.
    /// </summary>
    public class BasketService
    {
        private readonly DownloadService _downloadService;
        private readonly UserStatusService _statusService;

        private const string s_DefaultBasket = "default";
        private const string s_MyBasket = "My Basket";

        public event Action OnBasketChanged;
        public Basket CurrentBasket { get; set; }

        /// <summary>
        /// The list of selected images in the basket
        /// </summary>
        public List<Image> BasketImages { get; private set; } = new List<Image>();


        public BasketService( UserStatusService statusService, DownloadService downloadService)
        {
            _statusService = statusService;
            _downloadService = downloadService;
        }

        private void NotifyStateChanged()
        {
            OnBasketChanged?.Invoke();
        }

        /// <summary>
        /// Loads the selected images in the basket, and adds them to the in-memory
        /// SelectedImages collection. 
        /// </summary>
        public void LoadSelectedImages()
        {
            using var db = new ImageContext();
            var watch = new Stopwatch("GetSelectedImages");

            // TODO Assign current basket?
            var images = db.Baskets.Where( x => x.BasketId == CurrentBasket.BasketId )
                            .Include(x => x.BasketEntries)
                            .ThenInclude(x => x.Image)
                            .ThenInclude(x => x.Folder)
                            .SelectMany( x => x.BasketEntries )
                            .Select( x => x.Image )
                            .ToList();

            // We can't used ThenInclude to pull in the image tags due to this
            // but in the EF framework: https://github.com/dotnet/efcore/issues/19418
            // It's just too slow. So until they fix it (probably EF 5) we need
            // to manually explicitly load the tags for each image, which is
            // very quick.
            foreach (var img in images)
                db.LoadTags(img);

            watch.Stop();

            BasketImages.Clear();
            BasketImages.AddRange(images);

            NotifyStateChanged();
        }

        /// <summary>
        /// Clears the selection from the basket
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            using var db = new ImageContext();

            try
            {
                BasketImages.Clear();
                await db.BatchDelete( db.BasketEntries.Where( x => x.BasketId.Equals( CurrentBasket.BasketId ) ) );
                Logging.Log("Basket cleared.");

                NotifyStateChanged();

                _statusService.StatusText = "Basket selection cleared.";
            }
            catch (Exception ex)
            {
                Logging.LogError($"Error clearing basket: {ex.Message}");
            }
        }

        /// <summary>
        /// Return the baskets for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<Basket>> GetUserBaskets( AppIdentityUser user )
        {
            int userId = 0;

            if( user != null )
                userId = user.Id;

            using var db = new ImageContext();

            var myBaskets = await db.Baskets.Where(x => x.UserId == null || x.UserId == userId)
                                            .OrderBy( x => x.UserId == null ? 1 : 0 )
                                            .ThenBy( x => x.Name.ToLower() )
                                            .ToListAsync();

            if( userId != 0 && ! myBaskets.Any( x => x.UserId == userId ))
            {
                // Create a user basket if none exists.
                var userBasket = new Basket { Name = s_MyBasket, UserId = userId };
                db.Baskets.Add(userBasket);
                await db.SaveChangesAsync("SaveBasket");

                myBaskets.Insert(0, userBasket);
            }

            if( CurrentBasket == null )
            {
                SwitchBasket(s_MyBasket, user);
            }

            return myBaskets;
        }


        /// <summary>
        /// Async. Uses the download service to initiate a download of selected
        /// basket images, given a particular config - e.g., whether the images
        /// should be resized, watermarked, etc.
        /// </summary>
        /// <param name="config">Download configuration with size and watermark settings</param>
        /// <param name="keepPaths">True to keep folder structure, false for a flat zip of images.</param>
        /// <param name="OnProgress">Callback to give progress information to the UI</param>
        /// <returns>String path to the generated file, which is passed back to the doanload request</returns>
        /// TODO: Maybe move this elsewhere. 
        public async Task<string> DownloadSelection( ExportConfig config )
        {
            var images = BasketImages.Select(x => new FileInfo(x.FullPath)).ToArray();

            var virtualZipPath = await _downloadService.CreateDownloadZipAsync(images, config );

            if (!string.IsNullOrEmpty(virtualZipPath))
            {
                _statusService.StatusText = $"Basket selection downloaded to {virtualZipPath}.";

                return virtualZipPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Select or deselect an image - adding or removing it from the basket.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newState"></param>
        public async void SetBasketState( ICollection<Image> images, bool newState )
        {
            try
            {
                using var db = new ImageContext();
                bool changed = false;
                var watch = new Stopwatch("SetSelection");

                var existingEntries = db.BasketEntries.Where(x => x.BasketId == CurrentBasket.BasketId &&
                            images.Select(img => img.ImageId).Contains(x.ImageId));

                if (newState)
                {
                    // TODO: skip existing. Do we need this?!
                    var imagesToAdd = images.Where(img => !existingEntries.Select(x => x.ImageId).Contains( img.ImageId ) ).ToList();

                    var basketEntries = imagesToAdd.Select(img => new BasketEntry
                                {
                                    ImageId = img.ImageId,
                                    BasketId = CurrentBasket.BasketId,
                                    DateAdded = DateTime.UtcNow
                                }).ToList();

                    if (basketEntries.Any())
                    {
                        await db.BulkInsert(db.BasketEntries, basketEntries);

                        imagesToAdd.ForEach(img =>
                        {
                            img.BasketEntry = basketEntries.First(x => x.ImageId == img.ImageId);
                            BasketImages.Add(img);
                        });

                        changed = true;
                        _statusService.StatusText = $"Added {imagesToAdd.Count} image to the basket.";
                    }
                }
                else if (!newState)
                {
                    int deleted = await db.BatchDelete( existingEntries );
                    if( deleted > 0 )
                    {

                        images.ToList().ForEach(x => { x.BasketEntry = null; });
                        BasketImages.RemoveAll(x => images.Select(x => x.ImageId).Contains(x.ImageId));
                        changed = true;

                        _statusService.StatusText = $"Removed {deleted} images from the basket.";
                    }
                }

                watch.Stop();

                if (changed)
                    NotifyStateChanged();
            }
            catch( Exception ex )
            {
                Logging.LogError($"Unable to update the basket: {ex.Message}");
            }
        }

        public bool IsSelected(Image image)
        {
            return BasketImages.Any(x => x.ImageId == image.ImageId);
        }

        // TODO: Async
        public void CreateNewBasket( string name, int? userId )
        {
            using var db = new ImageContext();

            // TODO: check there isn't an existing basket with the same name and user?
            var newBasket = new Basket { Name = name, UserId = userId };
            db.Baskets.Add(newBasket);
            db.SaveChanges("SaveBasket");
        }

        public void SwitchBasket( string name, AppIdentityUser user )
        {
            using var db = new ImageContext();

            var watch = new Stopwatch("LoadBasket");
            Basket backupDefault = null;

            if (user != null)
            {                
                // First, look for a named basket belonging to this user
                var userBaskets = db.Baskets.Where(x => x.UserId.Equals( user.Id )).ToList();

                CurrentBasket = userBaskets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                backupDefault = userBaskets.FirstOrDefault(x => x.Name.Equals(s_MyBasket, StringComparison.OrdinalIgnoreCase));
            }

            if( CurrentBasket == null )
            {
                // Still haven't found it, so look for a named global baskets
                var globalBaskets = db.Baskets.Where(x => x.UserId == null).ToList();

                CurrentBasket = globalBaskets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (CurrentBasket == null)
                {
                    CurrentBasket = backupDefault;

                    if (CurrentBasket == null)
                    {
                        // Not found. Last resort - Pick first, alphabetically
                        CurrentBasket = db.Baskets.OrderBy(x => x.Name).FirstOrDefault();
                    }
                }
            }

            if (CurrentBasket != null)
            {
                LoadSelectedImages();
            }
            else
                Logging.LogError($"Unable to switch to basket {name}.");

            watch.Stop();
        }
    }
}
