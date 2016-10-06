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
    durations: DurationCriteria[]
}