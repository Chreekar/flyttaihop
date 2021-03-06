import * as React from 'react';
import { TraversalType, SearchResultItem } from '../../framework/models';

export class ResultsItem extends React.Component<SearchResultProps, void> {

    public render()
    {
        let res = this.props.searchResult;

        let durations = res.durations.map((item, index) =>
        {
            let travelsalTypeString = (item.type == TraversalType.biking ? 'cykla' : (item.type == TraversalType.commuting ? 'åka kommunalt' : 'gå'));

            //TODO: ikon på cykel/promenad/kollektivt)

            return <div key={ index } className="duration">
                { item.minutes } min ({ item.kilometers} km) att { travelsalTypeString } till { item.target.substring(0, item.target.lastIndexOf(",")) }
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
                    <span>{ res.address }</span>
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

export interface SearchResultProps
{
    searchResult: SearchResultItem;
}