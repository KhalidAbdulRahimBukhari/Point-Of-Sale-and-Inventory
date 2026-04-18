import api from "../../api/api";

const authStorage = {
  loginSuccess: (data: {
    token: string;
    userId: number;
    username: string;
    role: string;
  }) => {
    localStorage.setItem("authToken", data.token);
    localStorage.setItem("userId", String(data.userId));
    localStorage.setItem("username", String(data.username));
    localStorage.setItem("userRole", data.role);

    api.defaults.headers.common.Authorization = `Bearer ${data.token}`;
  },

  logout: () => {
    localStorage.clear();
    delete api.defaults.headers.common.Authorization;
  },

  getToken: () => localStorage.getItem("authToken"),
};

export default authStorage;
