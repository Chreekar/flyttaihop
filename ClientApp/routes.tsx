import * as React from 'react';
import { Router, Route, HistoryBase } from 'react-router';
import { Layout } from './components/Layout';
import { Criteria } from './components/Criteria';
import { Results } from './components/Results';
import { Saved } from './components/Saved';
import { Partner } from './components/Partner';

export default <Route component={ Layout }>
    <Route path='/' components={{ body: Criteria }} />
    <Route path='/results' components={{ body: Results }} />
    <Route path='/saved' components={{ body: Saved }} />
    <Route path='/partner' components={{ body: Partner }} />
</Route>;
