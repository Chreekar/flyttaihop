import * as React from 'react';
import { ApiServices } from '../../framework/services';
import { RentalResultItem, SortMode } from '../../framework/models';
import { RentalItem } from './RentalItem';

export class Rental extends React.Component<any, RentalState>
{
    constructor() {
        super();

        this.state = {
            searchResults: [],
            showShortTerm: false,
            sortMode: 'priceDesc'
        }

        ApiServices.Get<any[]>('/api/rental').then(data => {
            this.setState({
                searchResults: data
            });
        });
    }

    toggleFilter() {
        this.setState({
            showShortTerm: !this.state.showShortTerm
        });
    }

    changeSort(e: React.FormEvent) {
        this.setState({
            sortMode: (e.target as any).value as SortMode
        });
    }

    public render() {

        //TODO: skriv ut och sortera på avstånd när avståndsuppslagningen är implementerad

        let searchResults = this.state.searchResults
            .filter(item => item.Vanlig || (this.state.showShortTerm && item.Korttid))
            .sort((a, b) => {
                if (this.state.sortMode == 'priceDesc') {
                    return a.Hyra > b.Hyra ? -1 : 1
                }
                else if (this.state.sortMode == 'priceAsc') {
                    return a.Hyra < b.Hyra ? -1 : 1
                }
                /*else {
                    let totalKilometersA = 0;
                    a.durations.forEach(d => totalKilometersA += d.kilometers);
                    let totalKilometersB = 0;
                    b.durations.forEach(d => totalKilometersB += d.kilometers);
                    return totalKilometersA < totalKilometersB ? -1 : 1;
                }*/
            })
            .map((item, index) => {
                return <RentalItem key={index} rentalItem={item} />
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
                    <input type="checkbox" checked={this.state.showShortTerm} onChange={() => this.toggleFilter() } />&nbsp;
                    Visa även korttidskontrakt
                </p>
                <p>
                    Sortera på &nbsp;
                    <select value={this.state.sortMode} onChange={e => this.changeSort(e) }>
                        <option value="closest">Kortast avstånd</option>
                        <option value="priceDesc">Hyra fallande</option>
                        <option value="priceAsc">Hyra stigande</option>
                    </select>
                </p>
                <hr />
                {searchResults}
            </div>;
        }
    }
}

interface RentalState {
    searchResults?: RentalResultItem[];
    showShortTerm?: boolean;
    sortMode?: SortMode;
}