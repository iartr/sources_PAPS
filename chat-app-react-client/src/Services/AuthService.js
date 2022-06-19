import { isJwtExpired } from 'jwt-check-expiration';

export default class AuthService {

  static isLoggedIn() {
    let authToken = this.getAccessToken();
    return authToken != null && authToken !== "" && !isJwtExpired(authToken);
  }

  static getAccessToken() {
    let authToken = localStorage.getItem("authToken");
    return authToken;
  }

  static getUserInfo() {
    let userInfoJson = localStorage.getItem("userInfo");
    let userInfo = JSON.parse(userInfoJson);
    return userInfo;
  }

  static logout() {
    localStorage.removeItem("authToken");
    localStorage.removeItem("userInfo");
  }
}