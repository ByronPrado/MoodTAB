package com.mindraco.moodtab.viewmodel

import android.content.Context
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.mindraco.moodtab.data.AuthService
import com.mindraco.moodtab.data.SecurePrefsHelper
import kotlinx.coroutines.launch

class LoginViewModel(
    private val authService: AuthService,
    private val appContext: Context
) : ViewModel() {

    val nombre = MutableLiveData<String>()
    val email = MutableLiveData<String>()
    val esFamiliar = MutableLiveData(false)

    val errorMessage = MutableLiveData<String?>()
    val logsMessage = MutableLiveData<String?>()

    // Single-event navigation: expose destination
    private val _navigation = MutableLiveData<LoginDestination?>()
    val navigation: LiveData<LoginDestination?> = _navigation

    sealed class LoginDestination {
        object Main : LoginDestination()
        object UsuarioExterno : LoginDestination()
    }

    fun onLoginClicked() {
        viewModelScope.launch {
            val n = nombre.value.orEmpty()
            val e = email.value.orEmpty()
            logsMessage.value = "Intentando login con Nombre=$n, Email=$e"
            errorMessage.value = null

            val result = authService.login(n, e, esFamiliar.value == true)
            logsMessage.value = (logsMessage.value ?: "") + "\n" + result.log

            if (result.success && result.user != null) {
                // guarda en secure prefs
                SecurePrefsHelper.saveLoginData(appContext, result.user, esFamiliar.value == true)

                // Decide navegación
                if (esFamiliar.value == true) {
                    _navigation.value = LoginDestination.UsuarioExterno
                } else {
                    _navigation.value = LoginDestination.Main
                }
            } else {
                errorMessage.value = "Nombre o email inválidos."
            }
        }
    }

    fun doneNavigating() { _navigation.value = null }
}