import * as React from 'react';
import { Router, Route, HistoryBase } from 'react-router';
import { Layout } from './components/Layout';
import { Criterias } from './components/Criterias/Criterias';
import { Results } from './components/Results/Results';
import { SavedObjects } from './components/SavedObjects/SavedObjects';
import { Partner } from './components/Partner/Partner';
import { Rental } from './components/Rental/Rental';

export default <Route component={ Layout }>
    <Route path='/' components={{ body: Criterias }} />
    <Route path='/results' components={{ body: Results }} />
    <Route path='/saved' components={{ body: SavedObjects }} />
    <Route path='/partner' components={{ body: Partner }} />
    <Route path='/rental' components={{ body: Rental }} />
</Route>;
