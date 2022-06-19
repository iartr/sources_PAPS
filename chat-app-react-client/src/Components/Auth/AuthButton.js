import { useState } from 'react';
import { useHistory } from 'react-router-dom';
import AuthService from "../../Services/AuthService";
import { GOOGLE_CLIENT_ID } from '../../Constants';
import { GoogleLogin, GoogleLogout } from 'react-google-login';
import './Auth.css'

function AuthButton() {
	let history = useHistory();

	const [isLoggedIn, setIsLoggedIn] = useState(AuthService.isLoggedIn());

	function handleSuccess(response) {
		console.log("Successfully logged in with Google");
		localStorage.setItem("authToken", response.tokenId);
		
		let userInfo = {
			email: response.profileObj.email,
			name: response.profileObj.name,
			imageUrl: response.profileObj.imageUrl
		}

		let userInfoJson = JSON.stringify(userInfo);
		localStorage.setItem("userInfo", userInfoJson);
		
		history.push("/home");
		setIsLoggedIn(true);
	}

	function handleFailure (response) {
		console.log("Failure authorizing with Google");
		console.log(response);
	}

	function logout() {
		AuthService.logout();
		history.push('/login');
		setIsLoggedIn(false);
	}

	if (isLoggedIn) {
		return (
			<div className='logout' onClick={logout}>Выйти</div>
		);
	}
	else {
		return (
			<GoogleLogin
				clientId={GOOGLE_CLIENT_ID}
				buttonText="Войти через Google"
				onSuccess={handleSuccess}
				onFailure={handleFailure}
				cookiePolicy={'single_host_origin'}
			/>
		);
	}
}

export default AuthButton;