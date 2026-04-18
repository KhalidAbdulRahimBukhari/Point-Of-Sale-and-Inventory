// src/pages/Users/useUsers.ts
import { useEffect, useState } from "react";
import { UsersApi } from "./UsersApi";

export interface User {
  userId: number;
  userName: string;
  isActive: boolean;
  role: string;
  fullName: string;
  email: string | null;
  phone: string | null;
}

export interface CreateUserDto {
  userName: string;
  password: string;
  roleId: number;
  shopId: number;

  fullName: string;
  nationalNumber: string | null;
  email: string | null;
  dateOfBirth: string | null;
  countryID: number | null;
  gender: string | null;
  phone: string | null;
  imagePath: string | null;
}

export const EMPTY_USER: CreateUserDto = {
  userName: "",
  password: "",
  roleId: 1,
  shopId: 1,

  fullName: "",
  nationalNumber: null,
  email: null,
  dateOfBirth: null,
  countryID: null,
  gender: null,
  phone: null,
  imagePath: null,
};

export function useUsers() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [newUser, setNewUser] = useState<CreateUserDto>(EMPTY_USER);

  const loadUsers = async () => {
    try {
      setLoading(true);
      setUsers(await UsersApi.getAll());
    } catch {
      setError("Failed to load users");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

  const createUser = async () => {
    try {
      await UsersApi.create(newUser);
      setDialogOpen(false);
      setNewUser(EMPTY_USER);
      loadUsers();
    } catch {
      setError("Failed to create user");
    }
  };

  const deactivateUser = async (userId: number) => {
    if (userId == 1) {
      setError("Can Not deactivate the primary admin");
      return;
    }

    if (!window.confirm("Deactivate this user?")) return;
    try {
      await UsersApi.deactivate(userId);
      setUsers((users) =>
        users.map((u) => (u.userId === userId ? { ...u, isActive: false } : u)),
      );
    } catch {
      setError("Failed to deactivate user");
    }
  };

  const reactivateUser = async (userId: number) => {
    if (!window.confirm("Reactivate this user?")) return;

    try {
      await UsersApi.reactivate(userId);
      setUsers((users) =>
        users.map((u) => (u.userId === userId ? { ...u, isActive: true } : u)),
      );
    } catch {
      setError("Failed to reactivate user");
    }
  };

  const term = searchTerm.toLowerCase();

  const filteredUsers = users.filter(
    (u) =>
      (u.fullName ?? "").toLowerCase().includes(term) ||
      (u.userName ?? "").toLowerCase().includes(term),
  );

  return {
    users,
    filteredUsers,
    loading,
    error,
    searchTerm,
    dialogOpen,
    newUser,
    setSearchTerm,
    setDialogOpen,
    setNewUser,
    createUser,
    deactivateUser,
    reactivateUser,
  };
}

export const roleOptions = [
  { id: 1, name: "Admin" },
  { id: 2, name: "Cashier" },
];

export const countryOptions = [
  { id: 1, name: "USA" },
  { id: 2, name: "Canada" },
  { id: 3, name: "UK" },
];

export const genderOptions = ["Male", "Female", "Other"];
