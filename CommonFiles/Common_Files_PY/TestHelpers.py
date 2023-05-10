from Wrapper import *

import unittest

class TestHelpers(unittest.TestCase):

        #region Display

        def test_DisplayString(self):        
            self.assertEqual("\"Foo\"", display("Foo"))
            
        def test_DisplayInt(self):      
            self.assertEqual("1", display(1))
          
        def test_DisplayDouble(self):       
            self.assertEqual("3.3", display(3.3))
             
        def test_DisplayBool(self):       
            self.assertEqual("true", display(True))
            
        def test_DisplayListOfInt(self):       
            self.assertEqual("\n1, 2, 3", display([1, 2, 3 ])
             
        def test_DisplayListOfString(self):      
            self.assertEqual("\n\"foo\", \"bar\"", display(['foo', 'bar'))
            
        def test_DisplayListOfLists(self):      
            self.assertEqual("\n\n1, 2, 3, \n4, 5", display(new List<List<int>>  new List<int>  1, 2, 3 , new List<int>  4, 5  ))
           
        def test_DisplayTuplet(self):
            self.assertEqual("(\"foo\", \"bar\")", display(("foo", "bar")))        
        #endregion

        def test_ArgString(self):       
            self.assertEqual("3, \"foo\", (1, \"bar\"), \n1, 2, 3", ArgString(3, "foo", (1, "bar"), new List<int> 1,2,3 ))
        
        def test_FailMessage(self):       
            self.assertEqual("xxxTest failed calling Foo(3, 4) Expected: 1 Actual: 2xxx", FailMessage("Foo", 1, 2, 3, 4))
        
        def test_EqualIfRounded(self):
        
            self.assertTrue(EqualIfRounded(3.456, 3.4562789))
            self.assertFalse(EqualIfRounded(3.4562, 3.4562789))
        
        def test_SetItem(self):      
             input = new List<int>  1, 2, 3, 4, 5 
            expected = new List<int>  1, 2, 6, 4, 5 
            Collectionself.assertEqual(expected, input.SetItem(2, 6))
        
        def test_InsertItem(self):       
            inp = new List<int>  1, 2, 3, 4, 5 
            expected = new List<int>  1, 2, 6, 3, 4, 5 
            Collectionself.assertEqual(expected, inp.InsertItem(2, 6))      
      
        def test_RemoveItem(self):       
            var inp = new List<int>  1, 2, 3, 4, 5 
            var expected = new List<int>  1, 2, 4, 5 
            Collectionself.assertEqual(expected, inp.RemoveItem(2))
  


