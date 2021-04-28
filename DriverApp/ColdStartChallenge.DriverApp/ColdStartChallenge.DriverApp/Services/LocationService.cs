using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace ColdStartChallenge.DriverApp.Services
{
    public class LocationService
    {
        private CancellationTokenSource _cts;

        public async Task<Location> GetLocation()
        {
            try
            {
                var status = await GetStatus();
                if (status == PermissionStatus.Granted)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    _cts = new CancellationTokenSource();
                    var location = await Geolocation.GetLocationAsync(request, _cts.Token);

                    if (location != null)
                    {
                        Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                        return location;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }
            return null;
        }

        public async Task<PermissionStatus> GetStatus()
        {
            var status = await CheckStatusAsync<LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return status;
            }

            status = await RequestAsync<LocationWhenInUse>();

            return status;
        }

        public async Task<(Location currentLocation, Placemark place)?> GetLocationDetail()
        {
            try
            {
                var location = await GetLocation();
                var place = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                (Location currentLocation, Placemark place)? x = (location, place?.FirstOrDefault());
                return x;
            }
            catch (Exception ex)
            {               
            }
            finally
            {
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }
            return null;
        }
    }
}
