import React from 'react';
import { connect } from 'react-redux';

const Home = props => (
    <div>
        <h1>Welcome to Life Values</h1>
        <p>A play project to demonstrate Domain Driven Design (DDD) using event sourcing and CQRS.</p>
        <p>This project is using EventStore, MongoDB and ASP.NET Core 2.1</p>

    </div>
);

export default connect()(Home);
