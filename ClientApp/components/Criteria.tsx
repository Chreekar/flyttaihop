import * as React from 'react';
import { DistanceCriteria } from './DistanceCriteria';

export class Criteria extends React.Component<void, CriteriaState> {

    constructor() {
        super();
        
        this.state = {
            keywords: [],
            distanceCriterias: []
        };

        fetch("/api/criterias",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "GET"
            })
            .then(response => response.json())
            .then((data: CriteriaState) => {
                this.setState(data);
            });
    }

    updateKeywords(e: React.FormEvent) {
        this.setState({
            keywords: ((e.target as any).value as string).split(',').map(item => item.trim())
        });
    }

    addDistanceCriteria() {
        this.setState({
            distanceCriterias: [...this.state.distanceCriterias, {
                maxMinutes: 30,
                type: DistanceType.commuting,
                target: ''
            }]
        });
    }

    updateDistanceCriteria(oldItem: DistanceCriteriaState, newItem: DistanceCriteriaState) {
        this.setState({
            distanceCriterias: this.state.distanceCriterias.map((item, index) => item == oldItem ? newItem : item)
        });
    }

    removeDistanceCriteria(currentItem: DistanceCriteriaState) {
        this.setState({
            distanceCriterias: this.state.distanceCriterias.filter((item, index) => item != currentItem)
        });
    }

    save() {
        fetch("/api/criterias",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: "POST",
                body: JSON.stringify(this.state)
            })
            .then(response => response.json())
            .then((data: CriteriaState) => {
                this.setState(data);
            });
    }

    public render() {

        let distanceCriterias = this.state.distanceCriterias.map((item, index) => {
            return <DistanceCriteria key={ index } distanceCriteria={ item } updateDistanceCriteria={ this.updateDistanceCriteria.bind(this) } removeDistanceCriteria={ this.removeDistanceCriteria.bind(this) } />
        });

        return <div>
            <p>Söker bostadsrätter på minst 2, 5 rok i Stockholms län på minst 65 m2 till ett maxpris på 4 miljoner kr.</p>
            <div className="form-group">
                <label>Nyckelord</label>
                <input type="text" className="form-control" placeholder="uteplats, nyrenoverad..." value={ this.state.keywords.join(',') } onChange={ e => this.updateKeywords(e) }></input>
            </div>
            <div className="form-group">
                <label className="pull-left">Längsta avstånd</label>
                <button className="btn btn-default btn-xs" style={ { marginLeft: '5px' } } onClick={ () => this.addDistanceCriteria() }>+Lägg till</button>
                { distanceCriterias }
            </div>
            <div className="form-group">
                <div>
                    <button className="btn btn-primary" onClick={ () => this.save() }>Spara</button>
                </div>
            </div>
        </div >;
    }
}

interface CriteriaState {
    keywords?: string[];
    distanceCriterias?: DistanceCriteriaState[]
}

export interface DistanceCriteriaState {
    maxMinutes: number;
    type: DistanceType;
    target: string;
}

enum DistanceType {
    walking,
    biking,
    commuting
} 
