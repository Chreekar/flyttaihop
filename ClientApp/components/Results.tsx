import * as React from 'react';
import { SearchResult } from './SearchResult';
import { DurationCriteriaState } from './Criteria';

export class Results extends React.Component<any, ResultsState> {

    constructor() {
        super();

        this.state = {
            searchResults: []
        };

        fetch("/api/search",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "GET",
                credentials: 'same-origin'
            })
            .then(response => response.json())
            .then((data: SearchResultState[]) => {
                this.setState({
                    searchResults: data
                });
            });
    }

    public render() {

        let searchResults = this.state.searchResults.map((item, index) => {
            return <SearchResult key={ index } searchResult={ item } />
        });

        return <div>
            { searchResults }
        </div>;
    }

}

interface ResultsState {
    searchResults?: SearchResultState[];
}

export interface SearchResultState {
    area: string;
    city: string;
    address: string;
    price: string;
    fee: string;
    size: string;
    rooms: string;
    imageUrl: string;
    url: string;
    durations: DurationCriteriaState[]
}