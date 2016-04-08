using UnityEngine;
using System;
using System.Reflection;

public class ReflectionEvaluation : MonoBehaviour {
	
	private static BindingFlags SearchFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

	public enum InterfaceStatus {
		Missing,
		WrongSignature,
		IntegrityCheckFailed,
		Valid
	}

	//Evaluation
	public static InterfaceStatus EvaluateVariableStatus(Type classType, string name, Type requiredType) {
		// Check for the presence of the variable using reflection
		try {
			// Search for public variables on the class
			FieldInfo fieldInfo = classType.GetField(name, SearchFlags);
			
			// If the variable was found check the type
			if (fieldInfo.FieldType == requiredType) {
				return InterfaceStatus.Valid;
			}
			else {
				Debug.LogError("The variable " + name + " on the " + classType.Name + " has the wrong type.");
				return InterfaceStatus.WrongSignature;
			}
		}
		catch (NullReferenceException) {
			Debug.LogError("The variable " + name + " could not be found on the " + classType.Name + ".");
			return InterfaceStatus.Missing;
		}
	}
	
	public static InterfaceStatus EvaluatePropertyStatus(Type classType, string name, Type requiredType) {
		// Check for the presence of the property using reflection
		try {
			// Search for public properties on the class
			PropertyInfo propInfo = classType.GetProperty(name, SearchFlags);
			
			// If the property was found check the type
			if (propInfo.PropertyType == requiredType) {
				return InterfaceStatus.Valid;
			}
			else {
				Debug.LogError("The property " + name + " on the " + classType.Name + " has the wrong type.");
				return InterfaceStatus.WrongSignature;
			}
		}
		catch (NullReferenceException) {
			Debug.LogError("The property " + name + " could not be found on the " + classType.Name + ".");
			return InterfaceStatus.Missing;
		}
	}
	
	public static InterfaceStatus EvaluateConstructorStatus(Type classType, Type[] requiredParameters) {
		// Check for the presence of the constructor using reflection
		try {
			// Search for the constructor on the class
			ConstructorInfo constructorInfo = classType.GetConstructor(requiredParameters);
			
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			
			// Constructor has parameters?
			if (parameters.Length > 0) {
				// Constructor has parameters but none were expected
				if (requiredParameters == null) {
					Debug.LogError("A constructor for " + classType.Name + " does not match the required syntax.");
					return InterfaceStatus.WrongSignature;
				}
				
				// Wrong number of parameters?
				if (parameters.Length  != requiredParameters.Length) {
					Debug.LogError("A constructor for " + classType.Name + " does not match the required syntax.");
					return InterfaceStatus.WrongSignature;
				}
				
				// Test each parameter to make sure the type is correct
				for(int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex) {
					if (parameters[paramIndex].ParameterType != requiredParameters[paramIndex]) {
						Debug.LogError("A constructor for " + classType.Name + " does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
				}
			} // Constructor has no parameters?
			else {
				if ((requiredParameters != null) && (requiredParameters.Length > 0)) {
					Debug.LogError("A constructor for " + classType.Name + " does not match the required syntax.");
					return InterfaceStatus.WrongSignature;
				}
			}
			
			return InterfaceStatus.Valid;
		}
		catch (NullReferenceException) {
			// Build the parameter list
			string parameterList = "";
			foreach(Type type in requiredParameters) {
				if (parameterList.Length > 0) {
					parameterList += ", ";
				}
				parameterList += type.Name;
			}
			
			Debug.LogError("A constructor for " + classType.Name + " could not be found that took the following parameters: " + parameterList);
			return InterfaceStatus.Missing;
		}
	}
	
	public static InterfaceStatus EvaluateMethodStatus(Type classType, string name, Type requiredReturnType, 
	                                                   Type[] requiredParameters, bool[] isOutParameter) {
		// Check for the presence of the method using reflection
		try {
			// Search for the method on the class
			MethodInfo methodInfo = classType.GetMethod(name, SearchFlags);
			
			// If the method was found check the return type
			if (methodInfo.ReturnType == requiredReturnType) {
				ParameterInfo[] parameters = methodInfo.GetParameters();
				
				// Method has parameters?
				if (parameters.Length > 0) {
					// Method has parameters but none were expected
					if (requiredParameters == null) {
						Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
					
					// Wrong number of parameters?
					if (parameters.Length  != requiredParameters.Length) {
						Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
					
					// Test each parameter to make sure the type is correct
					for(int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex) {
						if (parameters[paramIndex].ParameterType != requiredParameters[paramIndex]) {
							Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
							return InterfaceStatus.WrongSignature;
						}
						
						if (parameters[paramIndex].IsOut != isOutParameter[paramIndex]) {
							Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
							return InterfaceStatus.WrongSignature;
						}
					}
				} // Method has no parameters?
				else {
					if ((requiredParameters != null) && (requiredParameters.Length > 0)) {
						Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
				}
				
				return InterfaceStatus.Valid;
			}
			else {
				Debug.LogError("The method " + name + " on the " + classType.Name + " does not match the required syntax.");
				return InterfaceStatus.WrongSignature;
			}
		}
		catch (NullReferenceException) {
			Debug.LogError("The method " + name + " could not be found on the " + classType.Name + ".");
			return InterfaceStatus.Missing;
		}
	}
}
