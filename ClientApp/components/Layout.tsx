import * as React from 'react';
import { Link } from 'react-router';

export class Layout extends React.Component<LayoutProps, void> {
    public render()
    {
        return <div className="container body-content">
            <div className="page-header">
                <h1>Flyttaihop.nu <small>Hitta bästa bostaden för båda två: -) </small></h1>
            </div>
            <ul className="nav nav-tabs">
                <li role="presentation">
                    <Link to={ '/' } activeClassName='active'>
                        Kriterier
                    </Link>
                </li>
                <li role="presentation">
                    <Link to={ '/results' } activeClassName='active'>
                        Sökresultat
                    </Link>
                </li>
                <li role="presentation">
                    <Link to={ '/saved' } activeClassName='active'>
                        Sparade objekt
                    </Link>
                </li>
                <li role="presentation">
                    <Link to={ '/partner' } activeClassName='active'>
                        Partner
                    </Link>
                </li>
            </ul>
            <div>
                { this.props.body }
            </div>
        </div>;
    }
}

export interface LayoutProps
{
    body: React.ReactElement<any>;
}
