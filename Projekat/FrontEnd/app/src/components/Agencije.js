import React, { useState, useEffect } from 'react';
import {Card, CardContent, Typography, Button, Grid, Divider, Dialog, DialogTitle, DialogContent, DialogActions, TextField,} from '@mui/material';
import { NavLink } from 'react-router-dom';

const Agencije = () => {
  const [agencije, setAgencije] = useState([]);
  const [openDodajForma, setOpenDodajForma] = useState(false);
  const [openIzmeniForma, setOpenIzmeniForma] = useState(false);
  const [novaAgencija, setNovaAgencija] = useState({});
  const [selectedAgencijaId, setSelectedAgencijaId] = useState(null);

  useEffect(() => {
    const fetchAgencije = async () => {
      try {
        const response = await fetch('https://localhost:7193/AgencijaContoller/PrezumiAgencije');
        const data = await response.json();
        setAgencije(data);
      } catch (error) {
        console.error('Error fetching agencije:', error);
      }
    };

    fetchAgencije();
  }, []);

  const handleObrisiAgenciju = async (id) => {
    try {
      await fetch(`https://localhost:7193/AgencijaContoller/ObrisiAgenciju/${id}`, {
        method: 'DELETE',
      });
      window.location.reload();
    } catch (error) {
      console.error('Error deleting agencija:', error);
    }
  };

  const handleDodajAgenciju = async () => {
    try {
      await fetch('https://localhost:7193/AgencijaContoller/DodajAgenciju', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(novaAgencija),
      });
      window.location.reload();
    } catch (error) {
      console.error('Error adding agencija:', error);
    }
  };

  const handleIzmeniAgenciju = async () => {
    try {
      await fetch(`https://localhost:7193/AgencijaContoller/AzurirajAgenciju/${selectedAgencijaId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(novaAgencija),
      });
      window.location.reload();
    } catch (error) {
      console.error('Error updating agencija:', error);
    }
  };

  const openIzmeniFormaHandler = (agencija) => {
    setSelectedAgencijaId(agencija.id);
    setNovaAgencija(agencija);
    setOpenIzmeniForma(true);
  };

  return (
    <div>
      <Typography variant="h6" sx={{ marginTop: '10px' }} gutterBottom>
        <Divider>Lista agencija</Divider>
        <Button
          id="dodajAgenciju"
          variant="contained"
          style={{ margin: '0 auto', display: 'block', marginTop: '10px' }}
          onClick={() => setOpenDodajForma(true)}
          sx={{ marginTop: '10px', backgroundColor: '#900C3F' }}
        >
          Dodaj agenciju
        </Button>
      </Typography>
      <Grid container spacing={2}>
        {agencije.map((agencija) => (
          <Grid item xs={12} sm={6} md={4} key={agencija.id}>
            <Card id="agencije" variant="outlined" sx={{ borderColor: 'purple', minWidth: '250px', margin: '1px' }}>
              <CardContent sx={{ paddingTop: '5px' }}>
                <Typography variant="h6">{agencija.naziv}</Typography>
                <Typography variant="body6" color="textSecondary">
                  Lokacija: {agencija.adresa}, {agencija.grad}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                  Email: {agencija.email}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                  Broj telefona: {agencija.brojTelefona}
                </Typography>
                <Divider>
                  <NavLink to={`/Agencije/${agencija.id}`}>
                    <Button
                      sx={{
                        color: '#900C3F',
                        transition: 'box-shadow 0.3s, color 0.3s',
                        '&:hover': {
                          boxShadow: '0 4px 8px rgba(0, 0, 0, 0.2)',
                          color: '#900C3F',
                        },
                      }}
                    >
                      Pregled putovanja
                    </Button>
                  </NavLink>
                </Divider>
                <Divider>
                  <Button id="obrisi" sx={{ color: '#B80010' }}  onClick={() => handleObrisiAgenciju(agencija.id)}>
                    Obriši
                  </Button>
                </Divider>
                <Divider>
                <Button
                  id="izmeni"
                  sx={{
                    color: '#B80000',
                    transition: 'box-shadow 0.3s, color 0.3s',
                    '&:hover': {
                      boxShadow: '0 4px 8px rgba(0, 0, 0, 0.2)',
                      color: '#B80000',
                    },
                  }}
                  onClick={() => openIzmeniFormaHandler(agencija)}
                >
                  Izmeni
                </Button>
                </Divider>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={openDodajForma} onClose={() => setOpenDodajForma(false)}>
        <DialogTitle>Dodaj agenciju</DialogTitle>
        <DialogContent>
          <TextField
            id="naziv"
            label="Naziv"
            variant="outlined"
            margin="normal"
            fullWidth
            onChange={(e) => setNovaAgencija({ ...novaAgencija, naziv: e.target.value })}
          />
          <TextField
            id="adresa"
            label="Adresa"
            variant="outlined"
            margin="normal"
            fullWidth
            onChange={(e) => setNovaAgencija({ ...novaAgencija, adresa: e.target.value })}
          />
          <TextField
            id="grad"
            label="Grad"
            variant="outlined"
            margin="normal"
            fullWidth
            onChange={(e) => setNovaAgencija({ ...novaAgencija, grad: e.target.value })}
          />
          <TextField
            id="brojTelefona"
            label="Broj telefona"
            variant="outlined"
            margin="normal"
            fullWidth
            onChange={(e) => setNovaAgencija({ ...novaAgencija, brojTelefona: e.target.value })}
          />
          <TextField
            id="email"
            label="Email"
            variant="outlined"
            margin="normal"
            fullWidth
            onChange={(e) => setNovaAgencija({ ...novaAgencija, email: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDodajForma(false)} color="secondary">
            Odustani
          </Button>
          <Button id="sacuvaj" onClick={handleDodajAgenciju} color="primary">
            Sačuvaj
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openIzmeniForma} onClose={() => setOpenIzmeniForma(false)}>
        <DialogTitle>Izmeni agenciju</DialogTitle>
        <DialogContent>
          <TextField
            id="nazivIzmeni"
            label="Naziv"
            variant="outlined"
            margin="normal"
            fullWidth
            value={novaAgencija.naziv || ''}
            onChange={(e) => setNovaAgencija({ ...novaAgencija, naziv: e.target.value })}
          />
          <TextField
            id="adresaIzmeni"
            label="Adresa"
            variant="outlined"
            margin="normal"
            fullWidth
            value={novaAgencija.adresa || ''}
            onChange={(e) => setNovaAgencija({ ...novaAgencija, adresa: e.target.value })}
          />
          <TextField
            id="gradIzmeni"
            label="Grad"
            variant="outlined"
            margin="normal"
            fullWidth
            value={novaAgencija.grad || ''}
            onChange={(e) => setNovaAgencija({ ...novaAgencija, grad: e.target.value })}
          />
          <TextField
            id="brojTelefonaIzmeni"
            label="Broj telefona"
            variant="outlined"
            margin="normal"
            fullWidth
            value={novaAgencija.brojTelefona || ''}
            onChange={(e) => setNovaAgencija({ ...novaAgencija, brojTelefona: e.target.value })}
          />
          <TextField
            id="emailIzmeni"
            label="Email"
            variant="outlined"
            margin="normal"
            fullWidth
            value={novaAgencija.email || ''}
            onChange={(e) => setNovaAgencija({ ...novaAgencija, email: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenIzmeniForma(false)} color="secondary">
            Odustani
          </Button>
          <Button id="sacuvajIzmene" onClick={handleIzmeniAgenciju} color="primary">
            Sačuvaj
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Agencije;
