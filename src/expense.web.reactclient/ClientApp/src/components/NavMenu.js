import React from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

const navmenu = props => (
    <Navbar inverse fixedTop fluid collapseOnSelect>
        <Navbar.Header>
            <Navbar.Brand>
                <Link to={'/'}>Life Values</Link>
            </Navbar.Brand>
            <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
            <Nav>
                <LinkContainer to={'/'} exact>
                    <NavItem><Glyphicon glyph='home' /> Home</NavItem>
                </LinkContainer>
                <LinkContainer to={'/values-list'}>
                    <NavItem><Glyphicon glyph='education' /> Values List</NavItem>
                </LinkContainer>
                { /*<LinkContainer to={'/fetchdata'}>
                    <NavItem><Glyphicon glyph='th-list' /> Fetch data</NavItem>
                </LinkContainer>*/}
            </Nav>
        </Navbar.Collapse>
    </Navbar>
);

export default navmenu;