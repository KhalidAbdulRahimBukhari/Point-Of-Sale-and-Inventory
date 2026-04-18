import {
  Box,
  Button,
  TextField,
  Typography,
  Container,
  Paper,
  Alert,
  InputAdornment,
  IconButton,
  CircularProgress,
  Checkbox,
  FormControlLabel,
} from "@mui/material";
import {
  Visibility,
  VisibilityOff,
  Person as PersonIcon,
} from "@mui/icons-material";
import { useLogin } from "./useLogin";

export default function LoginPage() {
  const { form, ui, updateField, togglePassword, submit } = useLogin();

  return (
    <Container maxWidth="xs">
      <Box mt={8}>
        <Paper sx={{ p: 4 }}>
          <Box display="flex" justifyContent="center" mb={2}>
            <PersonIcon fontSize="large" />
          </Box>

          <Typography variant="h5" align="center" mb={2}>
            Sign In
          </Typography>

          {ui.error && <Alert severity="error">{ui.error}</Alert>}

          <Box component="form" onSubmit={submit}>
            <TextField
              fullWidth
              label="Username"
              margin="normal"
              value={form.username}
              onChange={(e) => updateField("username", e.target.value)}
              disabled={ui.loading}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <PersonIcon />
                  </InputAdornment>
                ),
              }}
            />

            <TextField
              fullWidth
              label="Password"
              type={form.showPassword ? "text" : "password"}
              margin="normal"
              value={form.password}
              onChange={(e) => updateField("password", e.target.value)}
              disabled={ui.loading}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={togglePassword}>
                      {form.showPassword ? <Visibility /> : <VisibilityOff />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            <FormControlLabel
              control={
                <Checkbox
                  checked={form.rememberMe}
                  onChange={(e) => updateField("rememberMe", e.target.checked)}
                />
              }
              label="Remember username"
            />

            <Button
              fullWidth
              type="submit"
              variant="contained"
              sx={{ mt: 2 }}
              disabled={ui.loading}
            >
              {ui.loading ? <CircularProgress size={24} /> : "Login"}
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
