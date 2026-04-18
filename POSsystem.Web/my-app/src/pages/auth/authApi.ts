import api from "../../api/api";

export const authApi = {
  login: (username: string, password: string) =>
    api.post("/auth/login", { username, password }).then((res) => {
      localStorage.setItem("token", res.data.token);
      return res.data;
    }),
};
