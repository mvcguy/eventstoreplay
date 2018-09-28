import React from 'react';
import { Route } from 'react-router';
import Layout from './components/menu/Layout';
import Home from './components/home/Home';
import ValuesList from './components/values/Values-List';
import CreateValue from './components/values/create-value';

const layout = () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/values-list' component={ValuesList} />
        <Route path='/create-value' component={CreateValue} />
    </Layout>
);

export default layout;


