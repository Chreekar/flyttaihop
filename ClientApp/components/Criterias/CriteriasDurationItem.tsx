import * as React from 'react';
import { DurationCriteria } from '../../framework/models';

export class CriteriasDurationItem extends React.Component<CriteriasDurationItemProps, void> {

    updateMaxMinutes(item: DurationCriteria, e: React.FormEvent)
    {
        let newItem = {
            minutes: (e.target as any).value,
            type: item.type,
            target: item.target
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    updateTraversalType(item: DurationCriteria, e: React.FormEvent)
    {
        let newItem = {
            minutes: item.minutes,
            type: (e.target as any).value,
            target: item.target
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    updateTarget(item: DurationCriteria, e: React.FormEvent)
    {
        let newItem = {
            minutes: item.minutes,
            type: item.type,
            target: (e.target as any).value
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    public render()
    {

        let crit = this.props.durationCriteria;

        return <div className="duration-criteria">
            Max <input type="number" className="form-control" value={ crit.minutes ? crit.minutes.toString() : null } onChange={ e => this.updateMaxMinutes(crit, e) }></input> minuter att
            <select className="form-control" value={ crit.type.toString() } onChange={ e => this.updateTraversalType(crit, e) }>
                <option value="0">promenera</option>
                <option value="1">cykla</option>
                <option value="2">åka kommunalt</option>
            </select>
            till <input type="text" className="form-control" placeholder="gatuadress" value={ crit.target } onChange={ e => this.updateTarget(crit, e) }></input>
            <a href="javascript:void(0)" onClick={ e => this.props.removeDurationCriteria(crit) }>Radera</a>
        </div>;
    }

}

interface CriteriasDurationItemProps
{
    durationCriteria: DurationCriteria;
    updateDurationCriteria: (oldItem: DurationCriteria, newItem: DurationCriteria) => void;
    removeDurationCriteria: (currentItem: DurationCriteria) => void;
}