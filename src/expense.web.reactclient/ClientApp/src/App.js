import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import ValuesList from './components/Values-List';
import FetchData from './components/FetchData';

const layout = () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/values-list' component={ValuesList} />
        {/*<Route path='/fetchdata/:startDateIndex?' component={FetchData} />*/}
    </Layout>
);

export default layout;


