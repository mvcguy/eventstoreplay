import React from 'react';
import { Route } from 'react-router';
import Layout from './components/menu/Layout';
import Home from './components/home/Home';
import ValuesList from './components/values/Values-List';
import ManageValueRecord from './components/values/manage-value-record';

const layout = () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/values-list' component={ValuesList} />
        <Route path='/manage-value-record/:recordId?/:version?' component={ManageValueRecord} />
    </Layout>
);

export default layout;


