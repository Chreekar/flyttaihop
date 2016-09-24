import * as React from 'react';
import { DurationCriteria } from './DurationCriteria';

export class Criteria extends React.Component<void, CriteriaState> {

    constructor() {
        super();
        
        this.state = {
            keywords: [],
            durationCriterias: []
        };

        fetch("/api/criterias",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: 'GET',
                credentials: 'same-origin'
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

    addDurationCriteria() {
        this.setState({
            durationCriterias: [...this.state.durationCriterias, {
                minutes: 30,
                type: TraversalType.commuting,
                target: ''
            }]
        });
    }

    updateDurationCriteria(oldItem: DurationCriteriaState, newItem: DurationCriteriaState) {
        this.setState({
            durationCriterias: this.state.durationCriterias.map((item, index) => item == oldItem ? newItem : item)
        });
    }

    removeDurationCriteria(currentItem: DurationCriteriaState) {
        this.setState({
            durationCriterias: this.state.durationCriterias.filter((item, index) => item != currentItem)
        });
    }

    save() {
        fetch("/api/criterias",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: 'POST',
                credentials: 'same-origin',
                body: JSON.stringify(this.state)
            })
            .then(response => response.json())
            .then((data: CriteriaState) => {
                this.setState(data);
            });
    }

    public render() {

        let durationCriterias = this.state.durationCriterias.map((item, index) => {
            return <DurationCriteria key={ index } durationCriteria={ item } updateDurationCriteria={ this.updateDurationCriteria.bind(this) } removeDurationCriteria={ this.removeDurationCriteria.bind(this) } />
        });

        return <div>
            <p>Söker bostadsrätter på minst 2, 5 rok i Stockholms län på minst 65 m2 till ett maxpris på 4 miljoner kr.</p>
            <div className="form-group">
                <label>Nyckelord</label>
                <input type="text" className="form-control" placeholder="uteplats, nyrenoverad..." value={ this.state.keywords.join(',') } onChange={ e => this.updateKeywords(e) }></input>
            </div>
            <div className="form-group">
                <label className="pull-left">Längsta avstånd</label>
                <button className="btn btn-default btn-xs" style={ { marginLeft: '5px' } } onClick={ () => this.addDurationCriteria() }>+Lägg till</button>
                { durationCriterias }
            </div>
            <div className="form-group">
                <div>
                    <button className="btn btn-primary" onClick={ () => this.save() }>Spara</button>
                </div>
            </div>
        </div >;
    }
}

//TODO: Skapa mappar i ClientApp/components för varje flik och döp om till bättr ekomponentnamn
interface CriteriaState {
    keywords?: string[];
    durationCriterias?: DurationCriteriaState[]
}

//TODO: Bryt ut modellerna och döp till något annat än state eftersom den även används i sökresultatet
export interface DurationCriteriaState {
    minutes: number;
    type: TraversalType;
    target: string;
}

export enum TraversalType {
    walking,
    biking,
    commuting
} 
