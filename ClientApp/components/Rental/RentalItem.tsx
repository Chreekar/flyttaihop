import * as React from 'react';
import { TraversalType, RentalResultItem } from '../../framework/models';

export class RentalItem extends React.Component<RentalItemProps, void> {

    public render() {
        let res = this.props.rentalItem;

        /*let durations = res.durations.map((item, index) =>
        {
            let travelsalTypeString = (item.type == TraversalType.biking ? 'cykla' : (item.type == TraversalType.commuting ? 'åka kommunalt' : 'gå'));

            //TODO: ikon på cykel/promenad/kollektivt)

            return <div key={ index } className="duration">
                { item.minutes } min ({ item.kilometers} km) att { travelsalTypeString } till { item.target.substring(0, item.target.lastIndexOf(",")) }
            </div>;
        });*/

        return <div className="search-result">
            <div>
                <header>
                    <span>{ res.Hyra ? res.Hyra + ' kr/mån' : '-' }</span>
                    <span>{ res.Stadsdel + (res.Korttid ? ' - korttid' : '') + (res.Antal > 1 ? ' (' + res.Antal + ' st)' : '') }</span>
                    <span>{ res.Yta ? res.Yta + ' m2' : '-' }</span>
                </header>
                <div>
                    <span>{ res.AntalRum + ' rok' }</span>
                    <span>{ res.Gatuadress }</span>
                    <span>{ res.Vaning && res.Vaning > 0 ? res.Vaning + ' tr' : '-' }</span>
                </div>
                <footer>
                    { /*durations*/ }
                    <div>
                        <a className="btn btn-default" href={ 'https://bostad.stockholm.se' + res.Url } target="_blank">Besök på bostadsförmedlingen</a>
                        <button className="btn btn-default">Spara</button>
                    </div>
                </footer>
            </div>
        </div>;
    }

}

export interface RentalItemProps {
    rentalItem: RentalResultItem;
}