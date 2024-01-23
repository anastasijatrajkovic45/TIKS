import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './Navbar';
import Footer from './Footer';
import Pocetna from './components/Pocetna';
import Agencije from './components/Agencije';
import Putovanje from './components/Putovanje';
import PutovanjeProfil from './components/PutovanjeProfil';
import PutovanjePrikaz from './components/PutovanjePrikaz';
import Recenzije from './components/Recenzije';

const App = () => {
  return (
    <Router>
      <Navbar />

      <Routes>
        <Route path="/" element={<Pocetna />} />
        <Route path="/Agencije" element={<Agencije />} />
        <Route path="/Agencije/:id" element={<Putovanje />} />
        <Route path="/Putovanje/:id/Recenzije" element={<Recenzije />} />
        <Route path="/Putovanje/:id" element={<PutovanjeProfil />} />
        <Route path="/PutovanjePrikaz" element={<PutovanjePrikaz />} />
      </Routes>

      <Footer />
    </Router>
    
  );
};

export default App;