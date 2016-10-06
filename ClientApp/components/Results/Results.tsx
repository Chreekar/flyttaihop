import * as React from 'react';
import { ApiServices } from '../../framework/services';
import { ResultsItem } from './ResultsItem';
import { SearchResultItem } from '../../framework/models';

export class Results extends React.Component<any, ResultsState> {

    constructor()
    {
        super();

        this.state = {
            searchResults: []
        };

        ApiServices.Get<SearchResultItem[]>('/api/search').then(data =>
        {
            this.setState({
                searchResults: data
            });
        });
    }

    public render()
    {

        let searchResults = this.state.searchResults.map((item, index) =>
        {
            return <ResultsItem key={ index } searchResult={ item } />
        });

        return <div>
            { searchResults }
        </div>;
    }

}

interface ResultsState
{
    searchResults?: SearchResultItem[];
}