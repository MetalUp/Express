from Wrapper import *

import unittest

class TestHelpers(unittest.TestCase):

        #region Display

        def test_DisplayString(self):        
            self.assertEqual("'Foo'", display("Foo"))
            
        def test_DisplayInt(self):      
            self.assertEqual("1", display(1))
          
        def test_DisplayDouble(self):       
            self.assertEqual("3.3", display(3.3))
             
        def test_DisplayBool(self):       
            self.assertEqual("True", display(True))
            
        def test_DisplayListOfInt(self):       
            self.assertEqual("\n[1, 2, 3]", display([1, 2, 3 ]))
             
        def test_DisplayListOfString(self):      
            self.assertEqual("\n['foo', 'bar']", display(['foo', 'bar']))
            
        def test_DisplayListOfLists(self):      
            self.assertEqual("\n[\n[1, 2, 3], \n[4, 5]]", display([[1, 2, 3 ],[4, 5]]))
           
        def test_DisplayTuplet(self):
            self.assertEqual("('foo', 'bar')", display(("foo", "bar")))        
        #endregion

        #def test_ArgString(self):       
        #    self.assertEqual("", arg_string([3, "foo", (1, "bar"), [1,2,3] ]))
        
        def test_FailMessage(self):       
            self.assertEqual("xxxTest failed calling Foo(3, 4) Expected: 1 Actual: 2xxx", fail_message("Foo", 1, 2, [3, 4]))
        
        def test_EqualIfRounded(self):
        
            self.assertTrue(EqualIfRounded(3.456, 3.4562789))
            self.assertFalse(EqualIfRounded(3.4562, 3.4562789))
        
        #def test_SetItem(self):      
        #    inp = [1, 2, 3, 4, 5]
        #    expected = [1, 2, 6, 4, 5]
        #    self.assertEqual(expected, inp.SetItem(2, 6))
        
        #def test_InsertItem(self):       
        #    inp =[1, 2, 3, 4, 5 ]
        #    expected = [ 1, 2, 6, 3, 4, 5 ]
        #    self.assertEqual(expected, inp.InsertItem(2, 6))      
      
        #def test_RemoveItem(self):       
        #    inp =[1, 2, 3, 4, 5 ]
        #    expected = [ 1, 2, 4, 5 ]
        #    self.assertEqual(expected, inp.RemoveItem(2))
  
if __name__ == '__main__':
    unittest.main()

