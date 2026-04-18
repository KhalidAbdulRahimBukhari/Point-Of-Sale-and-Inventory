// src/api/api.ts
import api from "../../api/api";
import type { CreateUserDto } from "./useUsers";

// src/pages/Users/UsersApi.ts
export const UsersApi = {
  getAll: () => api.get("/users").then(res => res.data),

  create: (data: CreateUserDto) =>
    api.post("/users", data).then(res => res.data),

  deactivate: (userId: number) =>
    api.put(`/users/${userId}/deactivate`),

  reactivate: (userId: number) =>
    api.put(`/users/${userId}/activate`),
};
