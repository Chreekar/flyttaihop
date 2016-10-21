export interface Criteria
{
    keywords: Keyword[];
    durationCriterias: DurationCriteria[]
}

export interface Keyword
{
    text: string;
}

export interface DurationCriteria
{
    minutes: number;
    type: TraversalType;
    target: string;
}

export interface DurationAndDistance extends DurationCriteria
{
    kilometers: number;
}

export enum TraversalType
{
    walking,
    biking,
    commuting
}

export interface SearchResultItem
{
    area: string;
    city: string;
    address: string;
    price: string;
    fee: string;
    size: string;
    rooms: string;
    imageUrl: string;
    url: string;
    durations: DurationAndDistance[]
}