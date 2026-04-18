import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  MenuItem,
  Alert,
  CircularProgress,
} from "@mui/material";
import { Add, Delete, Search, Replay } from "@mui/icons-material";

import {
  useUsers,
  roleOptions,
  countryOptions,
  genderOptions,
} from "./useUsers";

export default function Users() {
  const {
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
  } = useUsers();

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" mb={3}>
        <Box>
          <Typography variant="h4" fontWeight="bold">
            Users
          </Typography>
          <Typography color="text.secondary">{users.length} users</Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => setDialogOpen(true)}
        >
          Add user
        </Button>
      </Box>

      <TextField
        fullWidth
        placeholder="Search users..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        InputProps={{ startAdornment: <Search /> }}
        sx={{ mb: 3 }}
      />

      {error && <Alert severity="error">{error}</Alert>}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Username</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Phone</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Status</TableCell>
              <TableCell />
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredUsers.map((user) => (
              <TableRow
                key={user.userId}
                sx={{ opacity: user.isActive ? 1 : 0.6 }}
              >
                <TableCell>{user.fullName}</TableCell>
                <TableCell>{user.userName}</TableCell>
                <TableCell>{user.email}</TableCell>
                <TableCell>{user.phone || "N/A"}</TableCell>
                <TableCell>
                  <Chip label={user.role} size="small" />
                </TableCell>
                <TableCell>
                  <Chip
                    label={user.isActive ? "Active" : "Inactive"}
                    size="small"
                    color={user.isActive ? "success" : "default"}
                  />
                </TableCell>
                <TableCell>
                  {user.isActive ? (
                    <IconButton
                      size="small"
                      color="error"
                      onClick={() => deactivateUser(user.userId)}
                    >
                      <Delete fontSize="small" />
                    </IconButton>
                  ) : (
                    <IconButton
                      size="small"
                      color="success"
                      onClick={() => reactivateUser(user.userId)}
                    >
                      <Replay fontSize="small" />
                    </IconButton>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {filteredUsers.length === 0 && (
        <Box textAlign="center" py={6}>
          <Typography color="text.secondary">No users found</Typography>
        </Box>
      )}

      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Add new user</DialogTitle>

        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} pt={2}>
            <TextField
              label="Full Name"
              required
              value={newUser.fullName}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, fullName: e.target.value }))
              }
            />

            <TextField
              label="National Number"
              value={newUser.nationalNumber ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({
                  ...p,
                  nationalNumber: e.target.value || null,
                }))
              }
            />

            <TextField
              label="Email"
              value={newUser.email ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, email: e.target.value || null }))
              }
            />

            <TextField
              type="date"
              label="Date of Birth"
              InputLabelProps={{ shrink: true }}
              value={newUser.dateOfBirth ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({
                  ...p,
                  dateOfBirth: e.target.value || null,
                }))
              }
            />

            <TextField
              select
              label="Country"
              value={newUser.countryID ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({
                  ...p,
                  countryID: e.target.value ? Number(e.target.value) : null,
                }))
              }
            >
              {countryOptions.map((c) => (
                <MenuItem key={c.id} value={c.id}>
                  {c.name}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              select
              label="Gender"
              value={newUser.gender ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, gender: e.target.value || null }))
              }
            >
              {genderOptions.map((g) => (
                <MenuItem key={g} value={g}>
                  {g}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              label="Phone"
              value={newUser.phone ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, phone: e.target.value || null }))
              }
            />

            <TextField
              label="Image URL"
              value={newUser.imagePath ?? ""}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, imagePath: e.target.value || null }))
              }
            />

            <TextField
              label="Username"
              required
              value={newUser.userName}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, userName: e.target.value }))
              }
            />

            <TextField
              type="password"
              label="Password"
              required
              value={newUser.password}
              onChange={(e) =>
                setNewUser((p) => ({ ...p, password: e.target.value }))
              }
            />

            <TextField
              select
              label="Role"
              required
              value={newUser.roleId}
              onChange={(e) =>
                setNewUser((p) => ({
                  ...p,
                  roleId: Number(e.target.value),
                }))
              }
            >
              {roleOptions.map((r) => (
                <MenuItem key={r.id} value={r.id}>
                  {r.name}
                </MenuItem>
              ))}
            </TextField>
          </Box>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={createUser}>
            Create User
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
