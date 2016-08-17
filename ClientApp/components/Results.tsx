import * as React from 'react';
import { SearchResult } from './SearchResult';

export class Results extends React.Component<any, ResultsState> {

    constructor() {
        super();

        this.state = {
            searchResults: []
        };

        fetch("/api/criterias/search",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "GET"
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
}