import React, { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  IconButton,
  Box,
  Menu,
  MenuItem,
  Divider,
  Chip,
} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";
import DashboardIcon from "@mui/icons-material/Dashboard";
import PeopleIcon from "@mui/icons-material/People";
import PointOfSaleIcon from "@mui/icons-material/PointOfSale";
import AssignmentReturnIcon from "@mui/icons-material/AssignmentReturn";
import InventoryIcon from "@mui/icons-material/Inventory";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import LogoutIcon from "@mui/icons-material/Logout";
import SettingsIcon from "@mui/icons-material/Settings";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";
import LinkedInIcon from "@mui/icons-material/LinkedIn";

const APP_VERSION = "1.0.0";
const DEVELOPER_NAME = "KHALID ABDUL RAHIM";
const LINKEDIN_URL =
  "https://www.linkedin.com/in/khalid-abdul-rahim-451943254/";

const DRAWER_WIDTH = 240;
const DRAWER_COLLAPSED = 64;

export default function DashboardLayout() {
  const [drawerOpen, setDrawerOpen] = useState(true);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const navigate = useNavigate();

  const handleLogout = () => {
    navigate("/login", { replace: true });
  };

  const drawerWidth = drawerOpen ? DRAWER_WIDTH : DRAWER_COLLAPSED;

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100dvh",
        width: "100vw",
        overflow: "hidden",
      }}
    >
      <AppBar
        position="static"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          flexShrink: 0,
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            onClick={() => setDrawerOpen(!drawerOpen)}
            edge="start"
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>

          <Typography variant="h6" sx={{ flexGrow: 1, fontWeight: 600 }}>
            Point Of Sale And Inventory Management System
          </Typography>

          <Chip
            label={`v${APP_VERSION}`}
            size="small"
            variant="outlined"
            sx={{
              mr: 2,
              color: "rgba(255,255,255,0.85)",
              borderColor: "rgba(255,255,255,0.4)",
              fontSize: "0.7rem",
            }}
          />

          <IconButton
            color="inherit"
            onClick={(e) => setAnchorEl(e.currentTarget)}
          >
            <AccountCircleIcon />
          </IconButton>

          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={() => setAnchorEl(null)}
          >
            <MenuItem onClick={() => navigate("/account")}>
              <ListItemIcon>
                <SettingsIcon fontSize="small" />
              </ListItemIcon>
              Account Settings
            </MenuItem>
            <Divider />
            <MenuItem onClick={handleLogout}>
              <ListItemIcon>
                <LogoutIcon fontSize="small" />
              </ListItemIcon>
              Logout
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      <Box
        sx={{
          display: "flex",
          flex: 1,
          minHeight: 0,
          overflow: "hidden",
        }}
      >
        <Box
          component="nav"
          sx={{
            width: drawerWidth,
            flexShrink: 0,
            transition: "width 0.25s ease",
          }}
        >
          <Drawer
            variant="permanent"
            sx={{
              "& .MuiDrawer-paper": {
                position: "relative",
                width: drawerWidth,
                transition: "width 0.25s ease",
                boxSizing: "border-box",
                overflowX: "hidden",
                display: "flex",
                flexDirection: "column",
              },
            }}
          >
            <List>
              <NavItem to="/dashboard" icon={<DashboardIcon />} label="Dashboard" open={drawerOpen} />
              <NavItem to="/cashier" icon={<PointOfSaleIcon />} label="Cashier" open={drawerOpen} />
              <NavItem to="/Invoices" icon={<ReceiptLongIcon />} label="Invoices" open={drawerOpen} />
              <NavItem to="/returns" icon={<AssignmentReturnIcon />} label="Returns" open={drawerOpen} />
              <NavItem to="/StockIn" icon={<InventoryIcon />} label="Stock In" open={drawerOpen} />
              <NavItem to="/users" icon={<PeopleIcon />} label="Users" open={drawerOpen} />
              <NavItem to="/login" icon={<LogoutIcon />} label="Logout" open={drawerOpen} />
            </List>

            {/* SIDEBAR LINKEDIN + NAME */}
            <Box
              sx={{
                mt: "auto",
                bottom: 0,
                width: "100%",
                borderTop: "1px solid",
                borderColor: "divider",
                p: 1,
                display: "flex",
                alignItems: "center",
                justifyContent: drawerOpen ? "flex-start" : "center",
                gap: 1,
              }}
            >
              <IconButton
                size="small"
                component="a"
                href={LINKEDIN_URL}
                target="_blank"
                rel="noopener noreferrer"
                color="primary"
              >
                <LinkedInIcon fontSize="small" />
              </IconButton>
              {drawerOpen && (
                <Typography variant="caption" color="text.secondary" noWrap>
                  {DEVELOPER_NAME}
                </Typography>
              )}
            </Box>
          </Drawer>
        </Box>

        <Box
          component="main"
          sx={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
            minWidth: 0,
            minHeight: 0,
            p: 3,
            overflow: "auto",
            bgcolor: "background.default",
          }}
        >
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}

function NavItem({
  to,
  icon,
  label,
  open,
}: {
  to: string;
  icon: React.ReactNode;
  label: string;
  open: boolean;
}) {
  return (
    <NavLink to={to} style={{ textDecoration: "none", color: "inherit" }}>
      {({ isActive }) => (
        <ListItemButton
          sx={{
            minHeight: 48,
            justifyContent: open ? "initial" : "center",
            px: 2.5,
            bgcolor: isActive ? "action.selected" : "transparent",
            "&:hover": {
              bgcolor: isActive ? "action.selected" : "action.hover",
            },
          }}
        >
          <ListItemIcon
            sx={{
              minWidth: 0,
              mr: open ? 2 : "auto",
              justifyContent: "center",
              color: isActive ? "primary.main" : "inherit",
            }}
          >
            {icon}
          </ListItemIcon>
          {open && (
            <ListItemText
              primary={label}
              sx={{
                "& .MuiTypography-root": {
                  fontWeight: isActive ? 600 : 400,
                },
              }}
            />
          )}
        </ListItemButton>
      )}
    </NavLink>
  );
}
