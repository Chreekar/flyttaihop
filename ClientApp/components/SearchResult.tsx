import * as React from 'react';
import { SearchResultState } from './Results';
import { TraversalType } from './Criteria';

export class SearchResult extends React.Component<SearchResultProps, void> {

    public render() {

        let res = this.props.searchResult;

        //TODO: Bryt ut till egen komponent?
        let durations = res.durations.map((item, index) => {

            let travelsalTypeString = (item.type == TraversalType.biking ? 'cykla' : (item.type == TraversalType.commuting ? 'åka kommunalt' : 'gå'));

            return <div className="duration">
                { item.minutes } minuter att { travelsalTypeString } till { item.target }
            </div>;
        });

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
                    { durations }
                    <div>
                        <a className="btn btn-default" href={ res.url } target="_blank">Besök på Hemnet</a>
                        <button className="btn btn-default">Spara</button>
                    </div>
                </footer>
            </div>
        </div>;
    }

}

export interface SearchResultProps {
    searchResult: SearchResultState;
}