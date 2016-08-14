import * as React from 'react';
import { DistanceCriteriaState } from './Criteria';

export class DistanceCriteria extends React.Component<DistanceCriteriaProps, void> {

    updateMaxMinutes(item: DistanceCriteriaState, e: React.FormEvent) {
        let newItem = {
            maxMinutes: (e.target as any).value,
            type: item.type,
            target: item.target
        };
        this.props.updateDistanceCriteria(item, newItem);
    }

    updateDistanceType(item: DistanceCriteriaState, e: React.FormEvent) {
        let newItem = {
            maxMinutes: item.maxMinutes,
            type: (e.target as any).value,
            target: item.target
        };
        this.props.updateDistanceCriteria(item, newItem);
    }

    updateTarget(item: DistanceCriteriaState, e: React.FormEvent) {
        let newItem = {
            maxMinutes: item.maxMinutes,
            type: item.type,
            target: (e.target as any).value
        };
        this.props.updateDistanceCriteria(item, newItem);
    }

    public render() {

        let crit = this.props.distanceCriteria;

        return <div className="distance-criteria">
            Max <input type="number" className="form-control" value={ crit.maxMinutes ? crit.maxMinutes.toString() : null } onChange={ e => this.updateMaxMinutes(crit, e) }></input> minuter att
            <select className="form-control" value={ crit.type.toString() } onChange={ e => this.updateDistanceType(crit, e) }>
                <option value="0">promenera</option>
                <option value="1">cykla</option>
                <option value="2">åka kommunalt</option>
            </select>
            till <input type="text" className="form-control" placeholder="gatuadress" value={ crit.target } onChange={ e => this.updateTarget(crit, e) }></input>
            <a href="javascript:void(0)" onClick={ e => this.props.removeDistanceCriteria(crit) }>Radera</a>
        </div>;
    }

}

export interface DistanceCriteriaProps {
    distanceCriteria: DistanceCriteriaState;
    updateDistanceCriteria: (oldItem: DistanceCriteriaState, newItem: DistanceCriteriaState) => void;
    removeDistanceCriteria: (currentItem: DistanceCriteriaState) => void;
}