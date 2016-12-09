import * as React from 'react';
import { ApiServices } from '../../framework/services';
import { ResultsItem } from './ResultsItem';
import { SearchResultItem } from '../../framework/models';

export class Results extends React.Component<any, ResultsState> {

    constructor() {
        super();

        this.state = {
            searchResults: [],
            showFailedLookupItems: false
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

    public render() {
        //TODO: Kunna sortera sökträffarna på pris, minsta medelavstånd (default)

        //TODO: Uppdatera till nya aspnetcore-spa-mallen som har Typescript 2.0

        let searchResults = this.state.searchResults
        .filter(item => this.state.showFailedLookupItems || item.durations.length > 0)
        .map((item, index) => {
            return <ResultsItem key={index} searchResult={item} />
        });

        return <div>
            <p>
                <input type="checkbox" checked={this.state.showFailedLookupItems} onChange={() => this.toggleFilter()} />&nbsp;
                Visa även objekt som inte kunnat avståndsbedömas
             </p>
             <hr />
            {searchResults}
        </div>;
    }

}

interface ResultsState {
    searchResults?: SearchResultItem[];
    showFailedLookupItems?: boolean;
}