package com.mindraco.moodtab.ui.login

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import com.mindraco.moodtab.R
import com.mindraco.moodtab.data.AuthService
import com.mindraco.moodtab.databinding.FragmentLoginBinding
import com.mindraco.moodtab.viewmodel.LoginViewModel
import com.mindraco.moodtab.viewmodel.LoginViewModelFactory

class LoginFragment : Fragment() {

    private var _binding: FragmentLoginBinding? = null
    private val binding get() = _binding!!
    private var direccion_ngrok = "0"


    private lateinit var viewModel: LoginViewModel

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentLoginBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        // Crear ViewModel usando Factory
        val authService = AuthService(direccion_ngrok)
        val factory = LoginViewModelFactory(authService, requireContext())
        viewModel = ViewModelProvider(this, factory).get(LoginViewModel::class.java)

        // Conectar binding con el viewModel y ciclo de vida
        binding.lifecycleOwner = viewLifecycleOwner
        binding.viewModel = viewModel

        // Observar navegación
        viewModel.navigation.observe(viewLifecycleOwner) { destination ->
            when (destination) {
                LoginViewModel.LoginDestination.Main -> {
                    findNavController().navigate(R.id.action_loginFragment_to_mainFragment)
                    viewModel.doneNavigating()
                }
                LoginViewModel.LoginDestination.UsuarioExterno -> {
                    findNavController().navigate(R.id.action_loginFragment_to_usuarioExternoFragment)
                    viewModel.doneNavigating()
                }
                else -> {}
            }
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
