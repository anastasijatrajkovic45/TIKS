import React from 'react';
import { AppBar, Toolbar, Typography, Tabs, Tab } from '@mui/material';
import { Link } from 'react-router-dom';

const NavBar = () => {
  return (
    <AppBar position="static" sx={{ backgroundColor: '#900C3F'}}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          TurističkiHub
        </Typography>
        <Tabs>
          <Tab label="Početna" component={Link} to="/" sx={{ color: 'white'}}/>
          <Tab label="Agencije" component={Link} to="/Agencije" sx={{ color: 'white' }}/>
          <Tab label="Destinacije" component={Link} to="/PutovanjePrikaz" sx={{ color: 'white' }}/>
          <Tab label="Kontakt" component={Link} to="/Kontakt" sx={{ color: 'white' }}/>
        </Tabs>
      </Toolbar>
    </AppBar>
  );
};

export default NavBar;