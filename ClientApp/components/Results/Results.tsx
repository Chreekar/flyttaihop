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
        //TODO: Kryssruta för att visa/dölja de som inte Google Maps lyckades hitta (så varje SearchResultItem behöver ha en property GoogleLookupSuccesful:boolean)

        //TODO: Kunna sortera sökträffarna på pris, minsta medelavstånd (default)

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