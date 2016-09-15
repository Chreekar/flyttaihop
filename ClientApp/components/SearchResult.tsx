import * as React from 'react';
import { SearchResultState } from './Results';

export class SearchResult extends React.Component<SearchResultProps, void> {

    public render() {

        let res = this.props.searchResult;

        return <div className="search-result">
            <img src={ res.imageUrl } />
            <div>
                <header>
                    <span>{ res.price }</span>
                    <span>{ res.area }</span>
                    <span>{ res.size }</span>
                </header>
                <div>
                    <span>{ res.fee }</span>
                    <span>{ res.city }</span>
                    <span>{ res.rooms }</span>
                </div>
                <footer>
                    <a className="btn btn-default" href={ res.url } target="_blank">Besök på Hemnet</a>
                    <button className="btn btn-default">Spara</button>
                </footer>
            </div>
        </div>;
    }

}

export interface SearchResultProps {
    searchResult: SearchResultState;
}