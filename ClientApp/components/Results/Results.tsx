import * as React from 'react';
import { ApiServices } from '../../framework/services';
import { ResultsItem } from './ResultsItem';
import { SearchResultItem } from '../../framework/models';

export class Results extends React.Component<any, ResultsState> {

    constructor() {
        super();

        this.state = {
            searchResults: [],
            showFailedLookupItems: false,
            sortMode: 'closest'
        };

        ApiServices.Get<SearchResultItem[]>('/api/search').then(data => {
            this.setState({
                searchResults: data
            });
        });
    }

    toggleFilter() {
        this.setState({
            showFailedLookupItems: !this.state.showFailedLookupItems
        });
    }

    changeSort(e: React.FormEvent) {
        this.setState({
            sortMode: (e.target as any).value as SortMode
        });
    }

    public render() {

        //TODO: Uppdatera till nya aspnetcore-spa-mallen som har Typescript 2.0

        let searchResults = this.state.searchResults
            .filter(item => this.state.showFailedLookupItems || item.durations.length > 0)
            .sort((a, b) => {
                if (this.state.sortMode == 'priceDesc') {
                    return a.price > b.price ? -1 : 1
                }
                else if (this.state.sortMode == 'priceAsc') {
                    return a.price < b.price ? -1 : 1
                }
                else {
                    let totalKilometersA = 0;
                    a.durations.forEach(d => totalKilometersA += d.kilometers);
                    let totalKilometersB = 0;
                    b.durations.forEach(d => totalKilometersB += d.kilometers);
                    return totalKilometersA < totalKilometersB ? -1 : 1;
                }
            })
            .map((item, index) => {
                return <ResultsItem key={index} searchResult={item} />
            });

        if (this.state.searchResults.length == 0) {
            return <div>
                <div className="spinner">
                    <div className="rect1"></div>
                    <div className="rect2"></div>
                    <div className="rect3"></div>
                    <div className="rect4"></div>
                    <div className="rect5"></div>
                </div>
            </div>;
        }
        else {
            return <div>
                <p>
                    <input type="checkbox" checked={this.state.showFailedLookupItems} onChange={() => this.toggleFilter()} />&nbsp;
                    Visa även objekt som inte kunnat avståndsbedömas
                </p>
                <p>
                    Sortera på &nbsp;
                    <select value={this.state.sortMode} onChange={e => this.changeSort(e)}>
                        <option value="closest">Kortast avstånd</option>
                        <option value="priceDesc">Pris fallande</option>
                        <option value="priceAsc">Pris stigande</option>
                    </select>
                </p>
                <hr />
                {searchResults}
            </div>;
        }
    }

}

interface ResultsState {
    searchResults?: SearchResultItem[];
    showFailedLookupItems?: boolean;
    sortMode?: SortMode;
}

type SortMode = 'priceDesc' | 'priceAsc' | 'closest';