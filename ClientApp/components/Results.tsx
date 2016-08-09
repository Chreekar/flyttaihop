import * as React from 'react';

export class Results extends React.Component<any, void> {

    /*constructor() {
        super();
        this.state = { forecasts: [], loading: true };

        fetch('/api/SampleData/WeatherForecasts') //TODO: Borde väl anropa denna i componentWillMount istället?
            .then(response => response.json())
            .then((data: WeatherForecast[]) => {
                this.setState({ forecasts: data, loading: false });
            });
    }*/

    //TODO: anropa nedan vid Hemnet-sök
    // http://www.hemnet.se/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2.5&living_area_min=65&location_ids%5B%5D=17744

    //TODO: använd nedan för att räkna ut avstånd
    // https://developers.google.com/maps/documentation/javascript/directions

    public render() {

        /*
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderForecastsTable(this.state.forecasts);
        */
        return <div>
            Results
        </div>;
    }

}