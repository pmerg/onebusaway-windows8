﻿using OneBusAway.DataAccess;
using OneBusAway.Model;
using OneBusAway.Model.BingService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBusAway.ViewModels
{
    /// <summary>
    /// View model for the search results page.
    /// </summary>
    public class SearchResultsViewModel : PageViewModelBase
    {
        private SearchResultsControlViewModel searchResultsControlViewModel;
        private MapControlViewModel mapControlViewModel;
        private ObaDataAccess obaDataAccess;

        /// <summary>
        /// Creates the search results page view model.
        /// </summary>
        public SearchResultsViewModel()
        {
            this.HeaderViewModel.SubText = "SEARCH RESULTS";

            this.SearchResultsControlViewModel = new SearchResultsControlViewModel();
            this.SearchResultsControlViewModel.RouteSelected += OnSearchResultsControlViewModelRouteSelected;
            this.SearchResultsControlViewModel.LocationSelected += OnSearchResultsControlViewModelLocationSelected;

            this.MapControlViewModel = new MapControlViewModel();
            this.MapControlViewModel.RefreshBusStopsOnMapViewChanged = false;            

            this.obaDataAccess = new ObaDataAccess();
        }        

        public SearchResultsControlViewModel SearchResultsControlViewModel
        {
            get
            {
                return this.searchResultsControlViewModel;
            }
            set
            {
                SetProperty(ref this.searchResultsControlViewModel, value);
            }
        }

        public MapControlViewModel MapControlViewModel
        {
            get
            {
                return this.mapControlViewModel;
            }
            set
            {
                SetProperty(ref this.mapControlViewModel, value);
            }
        }
        
        /// <summary>
        /// Searches asynchronously for a stop with the given name.
        /// </summary>
        public async Task SearchAsync(string queryText)
        {            
            await this.SearchResultsControlViewModel.Search(queryText, MapControlViewModel.UserLocation);
        }

        /// <summary>
        /// Called when the user selects a route's search result.
        /// </summary>
        private async void OnSearchResultsControlViewModelRouteSelected(object sender, RouteSelectedEventArgs e)
        {
            // If this is true then the user just clicked from a location-based search. Clear old bus stops!
            if (this.MapControlViewModel.RefreshBusStopsOnMapViewChanged)
            {
                this.MapControlViewModel.BusStops = null;
                this.MapControlViewModel.RefreshBusStopsOnMapViewChanged = false;
            }            

            var routes = await this.obaDataAccess.GetRouteDataAsync(e.RouteId);

            this.MapControlViewModel.BusStops = (from route in routes
                                                 from stop in route.Stops
                                                 select stop).ToList();

            this.MapControlViewModel.Shapes = (from route in routes
                                               from shape in route.Shapes
                                               select shape).ToList();

            this.MapControlViewModel.ZoomToRouteShape();
        }

        /// <summary>
        /// Called when the user selects a location as the search result
        /// </summary>
        /// <param name="sender">SearchResultsControlViewModel</param>
        /// <param name="e">SearchLocationResultViewModel</param>
        void OnSearchResultsControlViewModelLocationSelected(object sender, LocationSelectedEventArgs e)
        {
            if (!this.MapControlViewModel.RefreshBusStopsOnMapViewChanged)
            {
                this.MapControlViewModel.BusStops = null;
                this.MapControlViewModel.Shapes = null;
                this.MapControlViewModel.RefreshBusStopsOnMapViewChanged = true;
            }
            
            var point = new OneBusAway.Model.Point(e.Location.Point.Coordinates[0], e.Location.Point.Coordinates[1]);
            this.MapControlViewModel.UserLocation = point;
            this.MapControlViewModel.MapView = new MapView(point, ViewModelConstants.ZoomedInMapZoom, true);            
        }
    }
}
