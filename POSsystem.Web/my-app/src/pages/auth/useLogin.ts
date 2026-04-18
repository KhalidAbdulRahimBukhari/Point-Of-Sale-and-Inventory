import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { authApi } from "../../pages/auth/authApi";
import authStorage from "./authStorage";

export function useLogin() {
  const navigate = useNavigate();

  type LoginForm = {
    username: string;
    password: string;
    rememberMe: boolean;
    showPassword: boolean;
  };

  const [form, setForm] = useState<LoginForm>({
    username: "",
    password: "",
    rememberMe: false,
    showPassword: false,
  });

  const [ui, setUi] = useState({
    loading: false,
    error: "",
  });

  // Load remembered username
  useEffect(() => {
    const saved = localStorage.getItem("rememberedUsername");
    if (saved) {
      setForm((f) => ({ ...f, username: saved, rememberMe: true }));
    }
  }, []);

  const updateField = <K extends keyof LoginForm>(
    name: K,
    value: LoginForm[K],
  ) => {
    setForm((f) => ({ ...f, [name]: value }));
  };

  const togglePassword = () => {
    setForm((f) => ({ ...f, showPassword: !f.showPassword }));
  };

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setUi({ loading: false, error: "" });

    if (!form.username || !form.password) {
      setUi({ loading: false, error: "Username and password are required" });
      return;
    }

    try {
      setUi({ loading: true, error: "" });

      const res = await authApi.login(form.username, form.password);
      authStorage.loginSuccess(res);

      if (form.rememberMe) {
        localStorage.setItem("rememberedUsername", form.username);
      } else {
        localStorage.removeItem("rememberedUsername");
      }

      navigate("/dashboard");
    } catch {
      setUi({ loading: false, error: "Invalid username or password" });
    } finally {
      setUi((u) => ({ ...u, loading: false }));
    }
  };

  return {
    form,
    ui,
    updateField,
    togglePassword,
    submit,
  };
}
