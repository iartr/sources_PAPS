import './Header.css';
import React from 'react';
import Navbar from 'react-bootstrap/Navbar';
import AuthButton from './Auth/AuthButton';

class Header extends React.Component {

	render() {
		
		return (
		<Navbar className="bg-dark justify-content-between">
			<Navbar.Collapse className="justify-content-end">
				<AuthButton />
			</Navbar.Collapse>
		</Navbar>
		)
	}
}

export default Header;