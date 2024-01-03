import React, { useState, useEffect } from 'react';
import { Card, CardContent, Typography, Grid, Divider, Link, Button, TextField, DialogActions, DialogTitle, Dialog, DialogContent } from '@mui/material';
import { NavLink, useNavigate } from 'react-router-dom';
import { useParams } from 'react-router-dom';
import Putovanje from './Putovanje';
import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';

const Agencije = () => {
  const { id } = useParams();
  const [agencije, setAgencije] = useState([]);
  const [open, setOpen] = useState(false); 
  const [noviNaziv, setNoviNaziv] = useState('');
  const [novaAdresa, setNovaAdresa] = useState('');
  const [noviGrad, setNoviGrad] = useState('');
  const [noviEmail, setNoviEmail] = useState('');
  const [noviBrojTelefona, setNoviBrojTelefona] = useState('');
  const [trenutnaAgencija, setTrenutnaAgencija] = useState(null);
  const [dialogMode, setDialogMode] = useState(''); 
  const navigate = useNavigate();

  useEffect(() => {
    fetch('https://localhost:7193/AgencijaContoller/PrezumiAgencije')
      .then((response) => response.json())
      .then((data) => {
        setAgencije(data);
      })
      .catch((error) => {
        console.error('Greška prilikom preuzimanja agencija:', error);
      });
  }, []);

  const handleDodajAgenciju = () => {
    setDialogMode('Dodaj');
    setOpen(true);
  };

  const handleIzmeniAgenciju = (agencija) => {
    setTrenutnaAgencija(agencija);
    setNoviNaziv(agencija.naziv);
    setNovaAdresa(agencija.adresa);
    setNoviGrad(agencija.grad);
    setNoviEmail(agencija.email);
    setNoviBrojTelefona(agencija.brojTelefona);
    setDialogMode('Izmeni');
    setOpen(true);
  };
  

  const handleClose = () => {
    setOpen(false);
    setDialogMode('');
   
  };

const fetchData = () => {
  fetch('https://localhost:7193/AgencijaContoller/PrezumiAgencije')
    .then((response) => response.json())
    .then((data) => {
      setAgencije(data);
    })
    .catch((error) => {
      console.error('Greška prilikom preuzimanja agencija:', error);
    });
};

  const handleDialogAction = () => {
    if (dialogMode === 'Dodaj') {
      handleDodaj();
    } else if (dialogMode === 'Izmeni') {
      handleIzmeni();
    }
  };

  const [deletedAgencija, setDeletedAgencija] = useState(null);

  const handleObrisiAgenciju = (id) => {
    fetch(`https://localhost:7193/AgencijaContoller/ObrisiAgenciju/${id}`, {
      method: 'DELETE',
    })
      .then((response) => {
        if (response.ok) {
          const deleted = agencije.find((agencija) => agencija.id === id);
          setDeletedAgencija(deleted); 
          setOpen(true); 
          setAgencije(agencije.filter((agencija) => agencija.id !== id));
        } else {
          throw new Error('Neuspelo brisanje agencije');
        }
      })
      .catch((error) => {
        console.error('Greška prilikom brisanja agencije:', error);
      });
  };

  const handleDodaj = () => {
    if (noviNaziv && novaAdresa && noviGrad && noviEmail && noviBrojTelefona) {
      fetch('https://localhost:7193/AgencijaContoller/DodajAgenciju', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Naziv: noviNaziv,
          Adresa: novaAdresa,
          Grad: noviGrad,
          Email: noviEmail,
          BrojTelefona: noviBrojTelefona,
        }),
      })
        .then((response) => response.json())
        .then((data) => {
          handleClose();
          fetchData(); 
        })
        .catch((error) => {
          console.error('Greška prilikom dodavanja agencije:', error);
        });
    } else {
      console.error('Molimo Vas da unesete sve podatke.');
    }
  };
  
  const handleIzmeni = () => {
    if (
      trenutnaAgencija &&
      noviNaziv &&
      novaAdresa &&
      noviGrad &&
      noviEmail &&
      noviBrojTelefona
    ) {
      fetch(`https://localhost:7193/AgencijaContoller/AzurirajAgenciju/${trenutnaAgencija.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Naziv: noviNaziv,
          Adresa: novaAdresa,
          Grad: noviGrad,
          Email: noviEmail,
          BrojTelefona: noviBrojTelefona,
        }),
      })
        .then((response) => response.json())
        .then((data) => {
          handleClose();
          fetchData(); 
        })
        .catch((error) => {
          console.error('Greška prilikom izmene agencije:', error);
        });
    } else {
      console.error('Molimo Vas da unesete sve podatke.');
    }
  };

  return (
    <div>
      <Typography variant="h6" sx={{ marginTop: '25px' }} gutterBottom>
        <Divider>Lista agencija</Divider>
      </Typography>
      <Grid container spacing={2}>

         <Routes>
        {agencije.map((agencija) => (
          <Route key={agencija.id} path={`/Agencije/${agencija.id}`} element={<Putovanje />} />
        ))}
      </Routes>
        {agencije.map((agencija) => (
          <Grid item xs={12} sm={6} md={4} key={agencija.id}>
            <Card variant="outlined">
              <CardContent sx={{ paddingTop: '25px' }}>
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
                   
                <NavLink to={`/Agencije/${agencija.id}`}>
                  <Divider>
                    <Button>Pregled putovanja</Button>
                  </Divider>
                </NavLink>

                  <Divider>
                    <Button onClick={() => handleIzmeniAgenciju(agencija)}>Izmeni</Button>
                  </Divider>
      
                  <Divider>
                    <Button onClick={() => handleObrisiAgenciju(agencija.id)}>Obriši</Button>
                  </Divider>
                
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      {trenutnaAgencija && (
        <Dialog open={open} onClose={handleClose}>
          <DialogTitle>{dialogMode === 'Dodaj' ? 'Dodaj agenciju' : 'Izmeni agenciju'}</DialogTitle>
          <DialogContent>
            <TextField
              label="Naziv"
              value={noviNaziv || trenutnaAgencija.naziv}
              onChange={(e) => setNoviNaziv(e.target.value)}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Adresa"
              value={novaAdresa || trenutnaAgencija.adresa}
              onChange={(e) => setNovaAdresa(e.target.value)}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Grad"
              value={noviGrad || trenutnaAgencija.grad}
              onChange={(e) => setNoviGrad(e.target.value)}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Email"
              value={noviEmail || trenutnaAgencija.email}
              onChange={(e) => setNoviEmail(e.target.value)}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Broj Telefona"
              value={noviBrojTelefona || trenutnaAgencija.brojTelefona}
              onChange={(e) => setNoviBrojTelefona(e.target.value)}
              fullWidth
              margin="normal"
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Odustani</Button>
            <Button onClick={handleDialogAction} variant="contained" color="primary">
              {dialogMode === 'Dodaj' ? 'Dodaj agenciju' : 'Sačuvaj izmene'}
            </Button>
          </DialogActions>
        </Dialog>
      )}
      <Button variant="outlined" 
        style={{ borderColor: 'purple', margin: '0 auto', display: 'block', marginTop: '20px', }} 
        onClick={handleDodajAgenciju} sx={{ marginTop: '20px' }}>
        Dodaj agenciju
      </Button>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Dodaj Agenciju</DialogTitle>
        <DialogContent>
          <TextField
            label="Naziv"
            value={noviNaziv}
            onChange={(e) => setNoviNaziv(e.target.value)}
            fullWidth
            margin="normal"
          />
          <TextField
            label="Adresa"
            value={novaAdresa}
            onChange={(e) => setNovaAdresa(e.target.value)}
            fullWidth
            margin="normal"
          />
          <TextField
            label="Grad"
            value={noviGrad}
            onChange={(e) => setNoviGrad(e.target.value)}
            fullWidth
            margin="normal"
          />
          <TextField
            label="Email"
            value={noviEmail}
            onChange={(e) => setNoviEmail(e.target.value)}
            fullWidth
            margin="normal"
          />
          <TextField
            label="Broj Telefona"
            value={noviBrojTelefona}
            onChange={(e) => setNoviBrojTelefona(e.target.value)}
            fullWidth
            margin="normal"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Odustani</Button>
          <Button onClick={handleDodaj} variant="contained" color="primary">
            Dodaj agenciju
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Agencije;
