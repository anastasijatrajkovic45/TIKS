import React, { useEffect, useState } from 'react';
import { NavLink, useParams } from 'react-router-dom';
import {
  Typography,
  Divider,
  Button,
  Card,
  CardContent,
  Grid,
  TextField
} from '@mui/material';

const Putovanje = () => {
  const { id } = useParams();
  const [putovanja, setPutovanja] = useState([]);
  const [putovanjeZaIzmenu, setPutovanjeZaIzmenu] = useState(null);

  useEffect(() => {
    async function fetchPutovanja() {
      try {
        const response = await fetch(
          `https://localhost:7193/Putovanje/PreuzmiPutovanjaAgencije/${id}`
        );
        const data = await response.json();
        setPutovanja(data);
      } catch (error) {
        console.error('Greška prilikom dohvatanja putovanja:', error);
      }
    }
    fetchPutovanja();
  }, [id]);

const [izmenaPodataka, setIzmenaPodataka] = useState(false);
const [izmenjeniPodaci, setIzmenjeniPodaci] = useState({
  mesto: '',
  brojNocenja: 0,
  cena: 0,
  prevoz: ''
});

const [dodavanjeAktivno, setDodavanjeAktivno] = useState(false);
const [noviPodaci, setNoviPodaci] = useState({
  mesto: '',
  brojNocenja: 0,
  cena: 0,
  prevoz: ''
});

const handleDodajPutovanje = async () => {
  try {
    const response = await fetch(`https://localhost:7193/Putovanje/DodajPutovanjeAgenciji/${id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(noviPodaci),
    });
    if (response.ok) {
      console.log('Novo putovanje je dodato.');
      const newData = await response.json();
      setPutovanja([...putovanja, newData]);
      setDodavanjeAktivno(false);
      setNoviPodaci({});
    } else {
      throw new Error('Greška prilikom dodavanja putovanja.');
    }
  } catch (error) {
    console.error('Greška prilikom slanja zahteva za dodavanje putovanja:', error);
  }
};


const handleSacuvajIzmene = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(
        `https://localhost:7193/Putovanje/AzurirajPutovanje/${izmenjeniPodaci.id}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(izmenjeniPodaci),
        }
      );
      if (response.ok) {
        console.log(`Putovanje sa ID ${izmenjeniPodaci.id} je ažurirano.`);
        // Ažuriranje podataka u state-u
        setPutovanja((prevPutovanja) =>
          prevPutovanja.map((putovanje) =>
            putovanje.id === izmenjeniPodaci.id ? izmenjeniPodaci : putovanje
          )
        );
        setPutovanjeZaIzmenu(null); // Zatvaranje forme za izmenu
        setIzmenaPodataka(false);
      } else {
        throw new Error('Neuspelo ažuriranje putovanja.');
      }
    } catch (error) {
      console.error('Greška prilikom ažuriranja putovanja:', error);
    }
  };
  

  const handleIzmeniPutovanje = (putovanjeId) => {
    const izabranoPutovanje = putovanja.find((putovanje) => putovanje.id === putovanjeId);
    setPutovanjeZaIzmenu(izabranoPutovanje);
    setIzmenjeniPodaci(izabranoPutovanje); // Postavljanje podataka za uređivanje
    setIzmenaPodataka(true); // Prikaz forme za izmenu
  };
  

  const handleObrisiPutovanje = async (putovanjeId) => {
    try {
      const response = await fetch(
        `https://localhost:7193/Putovanje/ObrisiPutovanje/${putovanjeId}`,
        {
          method: 'DELETE',
        }
      );
      if (response.ok) {
        console.log(`Putovanje sa ID ${putovanjeId} je obrisano.`);
        setPutovanja(putovanja.filter((putovanje) => putovanje.id !== putovanjeId));
      } else {
        throw new Error('Neuspelo brisanje putovanja.');
      }
    } catch (error) {
      console.error('Greška prilikom brisanja putovanja:', error);
    }
  };

  return (
    <div style={{ textAlign: 'center', fontFamily: 'Roboto, sans-serif' }}>
      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif' }}>
        <Divider style={{ marginTop: '30px' }}>Lista putovanja</Divider>
      </Typography>
      <Grid container spacing={3} justifyContent="center">
        {putovanja.map((putovanje) => (
          <Grid item xs={12} sm={6} md={4} key={putovanje.id}>
            <Card variant="outlined">
              <CardContent>
                <Typography variant="h6">{putovanje.mesto}</Typography>
                <Typography variant="body6" color="textSecondary">
                  Broj nocenja: {putovanje.brojNocenja}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                  Cena: {putovanje.cena}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                  Prevoz: {putovanje.prevoz}
                </Typography>
                <Divider style={{ margin: '10px 0' }} />
                <NavLink to={`/Putovanje/${putovanje.id}`}>
                  <Button variant="contained" color="primary">
                    Pogledaj ponudu
                  </Button>
                </NavLink>
                <Divider style={{ margin: '10px 0' }} />
                <Button
                  variant="outlined"
                  color="secondary"
                  onClick={() => handleObrisiPutovanje(putovanje.id)}
                >
                  Obriši
                </Button>
                <Button
                  variant="outlined"
                  color="primary"
                  onClick={() => handleIzmeniPutovanje(putovanje.id)}
                  style={{ marginLeft: '10px' }}
                >
                  Izmeni
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      {putovanjeZaIzmenu && izmenaPodataka && (
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <Typography variant="h6">Izmeni putovanje</Typography>
        <form onSubmit={handleSacuvajIzmene} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <TextField
            label="Mesto"
            variant="outlined"
            value={izmenjeniPodaci.mesto}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, mesto: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            label="Prevoz"
            variant="outlined"
            value={izmenjeniPodaci.prevoz}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, prevoz: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            label="Broj noćenja"
            variant="outlined"
            type="number"
            value={izmenjeniPodaci.brojNocenja}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, brojNocenja: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            label="Cena"
            variant="outlined"
            type="number"
            value={izmenjeniPodaci.cena}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, cena: e.target.value })
            }
            style={{ marginBottom: '20px', width: '300px' }}
          />
          <Button type="submit" variant="contained" color="primary">
            Sačuvaj izmene
          </Button>
        </form>
      </div>
    )}
<Divider><Button
  variant="outlined"
  color="primary"
  onClick={() => setDodavanjeAktivno(true)}
  style={{ marginTop: '20px' }}
>
  Dodaj putovanje
</Button></Divider>


{dodavanjeAktivno && (
  <div style={{ textAlign: 'center', marginTop: '20px' }}>
    <Typography variant="h6">Dodaj putovanje</Typography>
    <form
      onSubmit={(e) => {
        e.preventDefault();
        handleDodajPutovanje();
      }}
      style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}
    >
      <TextField
        label="Mesto"
        variant="outlined"
        value={noviPodaci.mesto}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, mesto: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      <TextField
        label="Broj nocenja"
        variant="outlined"
        value={noviPodaci.brojNocenja}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, brojNocenja: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      <TextField
        label="Cena"
        variant="outlined"
        value={noviPodaci.cena}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, cena: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      <TextField
        label="Prevoz"
        variant="outlined"
        value={noviPodaci.prevoz}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, prevoz: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      <Button type="submit" variant="contained" color="primary">
        Dodaj putovanje
      </Button>
    </form>
  </div>
)}

    </div>
  );
};

export default Putovanje;





