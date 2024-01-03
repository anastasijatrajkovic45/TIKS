import React, { useState, useEffect } from 'react';
import {
  Typography,
  List,
  ListItem,
  ListItemText,
  TextField,
  Button,
  Box,
  Rating,
} from '@mui/material';

const RecenzijeList = () => {
  const [recenzije, setRecenzije] = useState([]);
  const [noviKomentar, setNoviKomentar] = useState('');
  const [novaOcena, setNovaOcena] = useState(0);

  useEffect(() => {
    const fetchRecenzije = async () => {
      try {
        const response = await fetch('https://localhost:7193/Recenzija/PrezumiRecenzije');
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();
        setRecenzije(data);
      } catch (error) {
        console.error('Greška prilikom preuzimanja recenzija:', error);
      }
    };

    fetchRecenzije();
  }, []);

  const handleDodajRecenziju = async () => {
    try {
      const novaRecenzija = { komentar: noviKomentar, ocena: novaOcena };
      const response = await fetch('https://localhost:7193/Recenzija/DodajRecenziju', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(novaRecenzija),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      // Osvježi prikaz recenzija
      const noviPodaci = [...recenzije, novaRecenzija];
      setRecenzije(noviPodaci);
      // Resetuj unos za novu recenziju
      setNoviKomentar('');
      setNovaOcena(0);
    } catch (error) {
      console.error('Greška prilikom dodavanja recenzije:', error);
    }
  };

  return (
    <Box>
      <Typography variant="h4">Recenzije</Typography>
      <List>
        {recenzije.map((recenzija) => (
          <ListItem key={recenzija.id}>
            {/* Prikaz podataka o recenziji */}
            <ListItemText
              primary={`Komentar: ${recenzija.komentar}`}
              secondary={`Ocena: ${recenzija.ocena}`}
            />
            {/* ... Dodajte ostale informacije koje želite prikazati ... */}
          </ListItem>
        ))}
      </List>

      <Typography variant="h6">Dodaj recenziju</Typography>
      <TextField
        label="Komentar"
        variant="outlined"
        value={noviKomentar}
        onChange={(e) => setNoviKomentar(e.target.value)}
        margin="normal"
        fullWidth
      />
      <Rating
        name="novaOcena"
        value={novaOcena}
        onChange={(event, newValue) => setNovaOcena(newValue)}
      />
      <Button variant="contained" color="primary" onClick={handleDodajRecenziju}>
        Dodaj recenziju
      </Button>
    </Box>
  );
};

export default RecenzijeList;
