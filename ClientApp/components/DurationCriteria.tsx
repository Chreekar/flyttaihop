import * as React from 'react';
import { DurationCriteriaState } from './Criteria';

export class DurationCriteria extends React.Component<DurationCriteriaProps, void> {

    updateMaxMinutes(item: DurationCriteriaState, e: React.FormEvent) {
        let newItem = {
            minutes: (e.target as any).value,
            type: item.type,
            target: item.target
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    updateTraversalType(item: DurationCriteriaState, e: React.FormEvent) {
        let newItem = {
            minutes: item.minutes,
            type: (e.target as any).value,
            target: item.target
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    updateTarget(item: DurationCriteriaState, e: React.FormEvent) {
        let newItem = {
            minutes: item.minutes,
            type: item.type,
            target: (e.target as any).value
        };
        this.props.updateDurationCriteria(item, newItem);
    }

    public render() {

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

export interface DurationCriteriaProps {
    durationCriteria: DurationCriteriaState;
    updateDurationCriteria: (oldItem: DurationCriteriaState, newItem: DurationCriteriaState) => void;
    removeDurationCriteria: (currentItem: DurationCriteriaState) => void;
}