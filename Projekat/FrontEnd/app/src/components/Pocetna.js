import React from 'react';
import { AppBar, Toolbar, Typography, Tabs, Tab, Container, Paper, Grid } from '@mui/material';
import { Link } from 'react-router-dom';
import Recenzije from './Rezencije';

const Pocetna = () => {
  return (
    <div>

      <Container sx={{ marginTop: 4 }}>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Paper elevation={3} sx={{ padding: 2, backgroundColor: '#F8DE22' }}>
              <Typography variant="h4" color="white" gutterBottom>
                Dobrodošli na našu aplikaciju putovanja!
              </Typography>
              <Typography variant="body1">
                Ovde ćete pronaći mnogo zabavnih stvari. Pregledajte našu ponudu i budite deo avanture!
              </Typography>
            </Paper>
          </Grid>
        </Grid>
      </Container>

      <Container>
        <Recenzije />
      </Container>

    </div>
  );
};

export default Pocetna;
