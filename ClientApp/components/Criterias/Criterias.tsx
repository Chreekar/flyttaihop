import * as React from 'react';
import { ApiServices } from '../../framework/services';
import { CriteriasDurationItem } from './CriteriasDurationItem';
import { Criteria, DurationCriteria, Keyword, TraversalType } from '../../framework/models';

export class Criterias extends React.Component<void, CriteriasState> {

    constructor()
    {
        super();

        this.state = {
            keywords: [],
            durationCriterias: []
        };

        ApiServices.Get<Criteria>('/api/criterias').then(data =>
        {
            this.setState({
                keywords: data.keywords,
                durationCriterias: data.durationCriterias
            });
        });
    }

    updateKeywords(e: React.FormEvent)
    {
        this.setState({
            keywords: ((e.target as any).value as string).split(',').map(item => ({ text: item.trim() }))
        });
    }

    addDurationCriteria()
    {
        this.setState({
            durationCriterias: [...this.state.durationCriterias, {
                minutes: 30,
                type: TraversalType.commuting,
                target: ''
            }]
        });
    }

    updateDurationCriteria(oldItem: DurationCriteria, newItem: DurationCriteria)
    {
        this.setState({
            durationCriterias: this.state.durationCriterias.map((item, index) => item == oldItem ? newItem : item)
        });
    }

    removeDurationCriteria(currentItem: DurationCriteria)
    {
        this.setState({
            durationCriterias: this.state.durationCriterias.filter((item, index) => item != currentItem)
        });
    }

    save()
    {
        let payload = {
            keywords: this.state.keywords,
            durationCriterias: this.state.durationCriterias
        };

        ApiServices.Post<Criteria>('/api/criterias', payload).then(data =>
        {
            this.setState({
                keywords: data.keywords,
                durationCriterias: data.durationCriterias
            });
        });
    }

    public render()
    { 
        let durationCriterias = this.state.durationCriterias.map((item, index) =>
        {
            return <CriteriasDurationItem key={ index } durationCriteria={ item } updateDurationCriteria={ this.updateDurationCriteria.bind(this) } removeDurationCriteria={ this.removeDurationCriteria.bind(this) } />
        });

        return <div>
            <p>Söker bostadsrätter på minst 2, 5 rok i Stockholms län på minst 65 m2 till ett maxpris på 4 miljoner kr.</p>
            <div className="form-group">
                <label>Nyckelord</label>
                <input type="text" className="form-control" placeholder="uteplats, nyrenoverad..." value={ this.state.keywords.map(x => x.text).join(',') } onChange={ e => this.updateKeywords(e) }></input>
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

interface CriteriasState
{
    keywords?: Keyword[];
    durationCriterias?: DurationCriteria[]
}
