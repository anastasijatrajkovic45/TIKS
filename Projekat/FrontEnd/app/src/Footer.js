import React from 'react';
import { Paper, Typography } from '@mui/material';

const Footer = () => {
  return (
    <Paper elevation={3} sx={{ backgroundColor: 'rgba(144, 12, 63, 0.9)', padding: 0.5, position: 'fixed', bottom: 0, width: '100%' }}>
      <Typography variant="body2" color="white" align="center">
        © 2024 TurističkiHub
      </Typography>
    </Paper>
  );
};

export default Footer;

