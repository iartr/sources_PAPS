import './NotLoggedIn.css';
import { useHistory } from "react-router-dom";
import AuthService from "../../Services/AuthService";
import Alert from 'react-bootstrap/Alert';

function NotLoggedIn() {

	let history = useHistory();
	let isLoggedIn = AuthService.isLoggedIn();
	if (isLoggedIn) {
		history.push('/home');
		return <div></div>
	}

	return <div className="not-logged-in-container">
	</div>
}

export default NotLoggedIn;